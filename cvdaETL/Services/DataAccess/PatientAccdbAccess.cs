using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using Dapper;
using System.Data.OleDb;

namespace cvdaETL.Services.DataAccess
{
    public class PatientAccdbAccess : IDbPatientAccess
    {

        private readonly string _connectionString;
        public PatientAccdbAccess()
        {
            _connectionString = Repo.Instance.ConnectionString;
        }

        public void InsertPatients(IEnumerable<ModelPatient> patients)
        {
            using (IDbConnection db = new OleDbConnection(_connectionString))
            {
                db.Open();

                string sql = @"
        INSERT INTO Patients (
            PatientID, EmisNo, ODSCode, NHSNumber, RiskScore, HouseBound, InHome, PCN, SurgeryName,
            UsualGP, Surname, FirstNames, Title, Sex, DateOfBirth, Age, HouseNameFlat, Street,
            Village, Town, County, PostCode, HomeTelephone, Mobile, WorkTelephone, Email,
            DeprivationDecile, HealthDecile, Ethnicity, PHMData, CurrentState
        )
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

                db.Execute(sql, patients.Select(patient => new
                {
                    patient.PatientID,
                    patient.EmisNo,
                    patient.ODSCode,
                    patient.NHSNumber,
                    patient.RiskScore,
                    patient.HouseBound,
                    patient.InHome,
                    patient.PCN,
                    patient.SurgeryName,
                    patient.UsualGP,
                    patient.Surname,
                    patient.FirstNames,
                    patient.Title,
                    patient.Sex,
                    patient.DateOfBirth,
                    patient.Age,
                    patient.HouseNameFlat,
                    patient.Street,
                    patient.Village,
                    patient.Town,
                    patient.County,
                    patient.PostCode,
                    patient.HomeTelephone,
                    patient.Mobile,
                    patient.WorkTelephone,
                    patient.Email,
                    patient.DeprivationDecile,
                    patient.HealthDecile,
                    patient.Ethnicity,
                    patient.PHMData,
                    patient.CurrentState
                }));

                db.Close();
            }
        }

        public void UpdatePatients(IEnumerable<ModelPatient> patients)
        {
            using (IDbConnection db = new OleDbConnection(_connectionString))
            {
                db.Open();

                string sql = @"
                UPDATE Patients SET
                    EmisNo = ?,
                    ODSCode = ?,
                    NHSNumber = ?,
                    RiskScore = ?,
                    HouseBound = ?,
                    InHome = ?,
                    PCN = ?,
                    SurgeryName = ?,
                    UsualGP = ?,
                    Surname = ?,
                    FirstNames = ?,
                    Title = ?,
                    Sex = ?,
                    DateOfBirth = ?,
                    Age = ?,
                    HouseNameFlat = ?,
                    Street = ?,
                    Village = ?,
                    Town = ?,
                    County = ?,
                    PostCode = ?,
                    HomeTelephone = ?,
                    Mobile = ?,
                    WorkTelephone = ?,
                    Email = ?,
                    DeprivationDecile = ?,
                    HealthDecile = ?,
                    Ethnicity = ?,
                    PHMData = ?,
                    CurrentState = ?
                WHERE NHSNumber = ?;";

                db.Execute(sql, patients.Select(patient => new
                {
                    patient.PatientID,
                    patient.EmisNo,
                    patient.ODSCode,
                    patient.NHSNumber,
                    patient.RiskScore,
                    patient.HouseBound,
                    patient.InHome,
                    patient.PCN,
                    patient.SurgeryName,
                    patient.UsualGP,
                    patient.Surname,
                    patient.FirstNames,
                    patient.Title,
                    patient.Sex,
                    patient.DateOfBirth,
                    patient.Age,
                    patient.HouseNameFlat,
                    patient.Street,
                    patient.Village,
                    patient.Town,
                    patient.County,
                    patient.PostCode,
                    patient.HomeTelephone,
                    patient.Mobile,
                    patient.WorkTelephone,
                    patient.Email,
                    patient.DeprivationDecile,
                    patient.HealthDecile,
                    patient.Ethnicity,
                    patient.PHMData,
                    patient.CurrentState
                }));

                db.Close();
            }
        }

        public Dictionary<string, string> GetNHSNumbers()
        {
            Dictionary<string, string> nhsNumbers = new Dictionary<string, string>();
            using var connection = new OleDbConnection(_connectionString);
            {
                connection.Open();

                string sql = "SELECT PatientID, NHSNumber FROM Patients;";
                nhsNumbers = connection.Query<(string, string)>(sql)
                    .ToDictionary(row => row.Item1, row => row.Item2);

                connection.Close();
            }

            return nhsNumbers;
        }
    }
}
