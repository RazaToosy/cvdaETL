using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using Serilog;

namespace cvdaETL.Services.DataAccess
{
    public class PatientDbAccess
    {
        private readonly string _connectionString;

        public PatientDbAccess()
        {
            _connectionString = Repo.Instance.ConnectionString;
        }

        public void InsertPatients(IEnumerable<ModelPatient> patients)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                db.Open();

                string sql = @"
            INSERT INTO Patients (
                PatientID, EmisNo, ODSCode, NHSNumber, RiskScore, HouseBound, InHome, PCN, SurgeryName,
                UsualGP, Surname, FirstNames, Title, Sex, DateOfBirth, Age, HouseNameFlat, Street,
                Village, Town, County, PostCode, HomeTelephone, Mobile, WorkTelephone, Email,
                DeprivationDecile, HealthDecile, Ethnicity, PHMData, CurrentState
            )
            VALUES (
                @PatientID, @EmisNo, @ODSCode, @NHSNumber, @RiskScore, @HouseBound, @InHome, @PCN, @SurgeryName,
                @UsualGP, @Surname, @FirstNames, @Title, @Sex, @DateOfBirth, @Age, @HouseNameFlat, @Street,
                @Village, @Town, @County, @PostCode, @HomeTelephone, @Mobile, @WorkTelephone, @Email,
                @DeprivationDecile, @HealthDecile, @Ethnicity, @PHMData, @CurrentState
            );";

                db.Execute(sql, patients);

                db.Close();
            }
        }

        public void UpdatePatients(IEnumerable<ModelPatient> patients)
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                db.Open();

                string sql = @"
                    UPDATE Patients SET
                        EmisNo = @EmisNo,
                        ODSCode = @ODSCode,
                        NHSNumber = @NHSNumber,
                        RiskScore = @RiskScore,
                        HouseBound = @HouseBound,
                        InHome = @InHome,
                        PCN = @PCN,
                        SurgeryName = @SurgeryName,
                        UsualGP = @UsualGP,
                        Surname = @Surname,
                        FirstNames = @FirstNames,
                        Title = @Title,
                        Sex = @Sex,
                        DateOfBirth = @DateOfBirth,
                        Age = @Age,
                        HouseNameFlat = @HouseNameFlat,
                        Street = @Street,
                        Village = @Village,
                        Town = @Town,
                        County = @County,
                        PostCode = @PostCode,
                        HomeTelephone = @HomeTelephone,
                        Mobile = @Mobile,
                        WorkTelephone = @WorkTelephone,
                        Email = @Email,
                        DeprivationDecile = @DeprivationDecile,
                        HealthDecile = @HealthDecile,
                        Ethnicity = @Ethnicity,
                        PHMData = @PHMData,
                        CurrentState = @CurrentState
                    WHERE NHSNumber = @NHSNumber;";

                db.Execute(sql, patients);

                db.Close();
            }
        }
        public Dictionary<string, string> GetNHSNumbers()
        {
            using (IDbConnection db = new SqliteConnection(_connectionString))
            {
                db.Open();

                string sql = "SELECT NHSNumber, PatientID FROM Patients;";

                Dictionary<string, string> nhsNumbers = db.Query<(string, string)>(sql)
                    .ToDictionary(row => row.Item1, row => row.Item2);

                db.Close();

                return nhsNumbers;
            }
        }

    }
}
