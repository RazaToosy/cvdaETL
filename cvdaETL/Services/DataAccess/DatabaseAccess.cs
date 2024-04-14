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
using Serilog;

namespace cvdaETL.Services.DataAccess
{
    public class DatabaseAccess
    {
        private readonly string _connectionString;

        public DatabaseAccess(string ConnectionString)
        {
            _connectionString = ConnectionString;
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
                DeprivationDecile, HealthDecile, Ethnicity, PHMData
            )
            VALUES (
                @PatientID, @EmisNo, @ODSCode, @NHSNumber, @RiskScore, @HouseBound, @InHome, @PCN, @SurgeryName,
                @UsualGP, @Surname, @FirstNames, @Title, @Sex, @DateOfBirth, @Age, @HouseNameFlat, @Street,
                @Village, @Town, @County, @PostCode, @HomeTelephone, @Mobile, @WorkTelephone, @Email,
                @DeprivationDecile, @HealthDecile, @Ethnicity, @PHMData
            );";

                db.Execute(sql, patients);

                db.Close();
            }
        }

    }
}
