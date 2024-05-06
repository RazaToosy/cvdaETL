using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using Dapper;
using Microsoft.VisualBasic;

namespace cvdaETL.Services.DataAccess.Accdb
{
    public class AppointmentsAndStaffAccdbAccess : IDbAppointmentsAccess
    {
        private string _connectionString;
        
        public AppointmentsAndStaffAccdbAccess()
        {
            _connectionString = Repo.Instance.ConnectionString;
        }

        public void InsertAppointments(List<ModelAppointment> Appointments)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();

                var sql = "INSERT INTO Appointments (AppointmentID, InteractionID, StaffID, PatientID, AppointmentDateTime, AppointmentTime, AppointmentPlannedTime, AppointmentActualTime, AppointmentLocation, AppointmentPostCode, AppointmentType, AppointmentMode, AppointmentOutcome, AppointmentFrequency) VALUES (@AppointmentID, @InteractionID, @StaffID, @PatientID, @AppointmentDateTime, @AppointmentTime, @AppointmentPlannedTime, @AppointmentActualTime, @AppointmentLocation, @AppointmentPostCode, @AppointmentType, @AppointmentMode, @AppointmentOutcome, @AppointmentFrequency);";

                connection.Execute(sql, Appointments.Select(a => new
                {
                   a.AppointmentID,
                   a.InteractionID,
                   a.StaffID,
                   a.PatientID,
                   a.AppointmentDateTime,
                   a.AppointmentTime,
                   a.AppointmentPlannedTime,
                   a.AppointmentActualTime,
                   a.AppointmentLocation,
                   a.AppointmentPostCode,
                   a.AppointmentType,
                   a.AppointmentMode,
                   a.AppointmentOutcome,
                   a.AppointmentFrequency
                }));

                connection.Close();
            }
        }

        public List<ModelAppointment> GetAllAppointments()
        {
            var appointments = new List<ModelAppointment>();
            using var connection = new OleDbConnection(_connectionString);
            {
                connection.Open();

                string sql = "SELECT * FROM Appointments;";
                appointments = connection.Query<ModelAppointment>(sql).ToList();

                connection.Close();
            }

            return appointments;
        }

        public void InsertStaff(ModelStaff Staff)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                var sql =
                    "INSERT INTO Staff (StaffID, StaffName, StaffRole) VALUES (@StaffID, @StaffName, @StaffRole);";

                connection.Execute(sql, new
                {
                    Staff.StaffID,
                    Staff.StaffName,
                    Staff.StaffRole
                });

            }
        }

        public Dictionary<string, string> GetStaffWithIDs()
        {
            Dictionary<string, string> staffWithIDs = new Dictionary<string, string>();
            using var connection = new OleDbConnection(_connectionString);
            {
                connection.Open();

                string sql = "SELECT StaffID, StaffName FROM Staff;";
                staffWithIDs = connection.Query<(string, string)>(sql)
                    .ToDictionary(row => row.Item1, row => row.Item2);

                connection.Close();
            }

            return staffWithIDs;
        }

        public List<ModelStaff> GetStaffWithRoles()
        {
            var staff= new List<ModelStaff>();
            using var connection = new OleDbConnection(_connectionString);
            {
                connection.Open();

                string sql = "SELECT * FROM Staff;";
                staff = connection.Query<ModelStaff>(sql).ToList();

                connection.Close();
            }

            return staff;
        }

        public List<DateTime> GetDatesOfAppointments()
        {
            int count = 0;
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                // SQL query to check if the date exists in the InsertDate field
                string query = "SELECT AppointmentDateTime FROM Appointments";
                var datesWithTime = connection.Query<DateTime>(query).ToList();
                return RemoveTime(datesWithTime);
            }
            return null;
        }

        private List<DateTime> RemoveTime(List<DateTime> dateTimes)
        {
            var result = new List<DateTime>();
            foreach (var dateTime in dateTimes)
            {
                result.Add(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day));
            }
            return result;
        }
    }
}
