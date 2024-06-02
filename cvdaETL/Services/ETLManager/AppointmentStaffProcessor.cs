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
            if (!File.Exists(Path.Combine(Repo.Instance.CsvPath, "ClinicsAppointments.csv"))) return;
            var appointments = new CsvHelperManager().ImportFromCsv<ModelAppointment, AppointmentStaffMap>(Path.Combine(Repo.Instance.CsvPath, "ClinicsAppointments.csv"));
            _currentStaff = _dbAccess.AppointmentsAccess.GetStaffWithIDs();

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
                _dbAccess.AppointmentsAccess.InsertStaff(teamMember);
            });
            
            Repo.Instance.CvdaStaff = _dbAccess.AppointmentsAccess.GetStaffWithRoles();
            
            //Ignore Booked Outcome as will have to handle deletions of appointments which is out of scope for this project.
            //var pastAppointments = appointments.Where(a => a.AppointmentOutcome != "Booked").ToList();
            //var existingDates = _dbAccess.AppointmentsAccess.GetDatesOfAppointments();
            //var newAppointments = pastAppointments.Where(a => !existingDates.Contains(a.AppointmentDateTime)).ToList();
            
            _dbAccess.AppointmentsAccess.DeleteAppointments();

            var patientsWithNHSNo = _dbAccess.PatientAccess.GetNHSNumbers();

            appointments.ForEach(appointment =>
            {
                appointment.AppointmentID = Guid.NewGuid().ToString();
                appointment.StaffID = Repo.Instance.CvdaStaff.FirstOrDefault(x => x.StaffName == appointment.StaffName).StaffID;
                appointment.PatientID = patientsWithNHSNo.FirstOrDefault(x => x.Value == appointment.NHSNumber).Key;
                appointment.InteractionID = (Repo.Instance.CvdaInteractions
                    .FirstOrDefault(x => x.PatientID == appointment.PatientID)?.InteractionID
                    .ToString()) ?? string.Empty;
                appointment.AppointmentDateTime = appointment.AppointmentDateTime.Add(TimeSpan.Parse(appointment.AppointmentTime));
                appointment.AppointmentMode = appointment.AppointmentType.Contains("Tel") | appointment.AppointmentType.Contains("Remote") ? "Telephone" : "F2F";
                appointment.AppointmentFrequency = appointment.AppointmentType.Contains("Follow") ? "FollowUp" : "New";
            });

            _dbAccess.AppointmentsAccess.InsertAppointments(appointments);
            
            Repo.Instance.CvdaAppointments = _dbAccess.AppointmentsAccess.GetAllAppointments();
        }
    }
}
