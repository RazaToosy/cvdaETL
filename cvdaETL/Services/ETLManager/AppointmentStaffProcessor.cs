using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Maps;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using cvdaETL.Services.CsvHelper;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Services.ETLManager
{
    public class AppointmentStaffProcessor
    {
        ResolveDb _dbAccess;
        private Dictionary<string, string> _currentStaff;

        public AppointmentStaffProcessor(ResolveDb dbAccess)
        {
            _dbAccess = dbAccess;
            _currentStaff = new Dictionary<string, string>();
        }

        public void ImportAppointmentsAndStaff()
        {
            var appointments = new CsvHelperManager().ImportFromCsv<ModelAppointment, AppointmentStaffMap>(Path.Combine(Repo.Instance.CsvPath, "ClinicsAppointments.csv"));
            _currentStaff = _dbAccess.AppointmentAccess.GetStaffWithIDs();

            // Extract RecallTeam names from imported interactions and find names not in existingRecallTeamNames
            var StaffNamesNamesNotInExisting = appointments
                .Select(i => new { i.StaffName, i.StaffRole })
                .Distinct()
                .Where(staff => !_currentStaff.ContainsValue(staff.StaffName))
                .GroupBy(staff => staff.StaffName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(g => g.StaffRole).First() // Or use another strategy to resolve duplicates
                );

            StaffNamesNamesNotInExisting.ToList().ForEach(kvp =>
            {
                var teamMember = new ModelStaff
                {
                    StaffID = Guid.NewGuid().ToString(),
                    StaffName = kvp.Key,
                    StaffRole = kvp.Value
                };

                // Insert the RecallTeam into the database
                _dbAccess.AppointmentAccess.InsertStaff(teamMember);
            });
            
            Repo.Instance.CvdaStaff = _dbAccess.AppointmentAccess.GetStaffWithRoles();
            
            //Ignore Booked Outcome as will have to handle deletions of appointments which is out of scope for this project.
            var pastAppointments = appointments.Where(a => a.AppointmentOutcome != "Booked").ToList();
            
            var existingDates = _dbAccess.AppointmentAccess.GetDatesOfAppointments();
            
            var newAppointments = pastAppointments.Where(a => !existingDates.Contains(a.AppointmentDateTime)).ToList();

            newAppointments.ForEach(appointment =>
            {
                appointment.AppointmentID = Guid.NewGuid().ToString();
                appointment.StaffID = _currentStaff.FirstOrDefault(x => x.Value == appointment.StaffName).Key;
                appointment.PatientID = Repo.Instance.PatientIDsNHSNumber.FirstOrDefault(x => x.Value == appointment.NHSNumber).Key;
                appointment.InteractionID = (Repo.Instance.CvdaInteractions
                    .FirstOrDefault(x => x.PatientID == appointment.PatientID)?.InteractionID
                    .ToString()) ?? "No Interaction Found";
                appointment.AppointmentDateTime = appointment.AppointmentDateTime.Add(TimeSpan.Parse(appointment.AppointmentTime));
                appointment.AppointmentMode = appointment.AppointmentType.Contains("Tel") ? "Telephone" : "F2F";
                appointment.AppointmentFrequency = appointment.AppointmentType.Contains("Follow") ? "FollowUp" : "New";
            });

            _dbAccess.AppointmentAccess.InsertAppointments(newAppointments);
            
            Repo.Instance.CvdaAppointments = _dbAccess.AppointmentAccess.GetAllAppointments();
            //public string AppointmentID { get; set; }  - new Guid
            //public string InteractionID { get; set; } - from Interactions. Work out from patientID and Date of CvdaInteracitons
            //public string StaffID { get; set; } - from Staff. Work out from CvdaStaff from StaffName
            //public string PatientID { get; set; } - from Repo.PatientsWithIDs
            //public string NHSNumber { get; set; } - already there
            //public string StaffName { get; set; } - already there
            //public string StaffRole { get; set; } - already there
            //public DateTime AppointmentDateTime { get; set; } - join this with the AppointmentTime to get the correct DateTime
            //public string AppointmentTime { get; set; } - already there
            //public int AppointmentPlannedTime { get; set; } - already there
            //public int AppointmentActualTime { get; set; } - already there
            //public string AppointmentLocation { get; set; } - already there
            //public string AppointmentPostCode { get; set; } - already there
            //public string AppointmentType { get; set; } - already there - from SlotType
            //public string AppointmentMode { get; set; } - F2F or Telephone. If SlotType contains Tel then Telephone otherwise assume it's face to face and add "F2F or "Telephone" to the AppointmentMode
            //public string AppointmentOutcome { get; set; } - already there
            //public string AppointmentFrequency { get; set; } - If the Type contains FollowUp then it is a follow up appointment otherwise it is a new appointment.


            //AppointmentMode you work out from the SlotType. If contains Tel then it is a telephone appointment.

            //Need to add Time to Date to get the correct DateTime for the appointment.

        }
    }
}
