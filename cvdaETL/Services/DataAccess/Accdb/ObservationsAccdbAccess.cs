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
    public class ObservationsAccdbAccess : IDbObservationsAccess
    {
        private readonly string _connectionString;

        public ObservationsAccdbAccess()
        {
            _connectionString = Repo.Instance.ConnectionString;
        }
        
        public void InsertObservations(List<ModelObservation> Observations)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();

                var sql = "INSERT INTO Observations (ObservationID, PatientID, AppointmentID, ObservationDate, ObservationCodeTerm, ObservationText, ObservationValue, ObservationValue2, ObservationTag, ObservationCheckSum, ObservationType) VALUES (@ObservationID, @PatientID, @AppointmentID, @ObservationDate, @ObservationCodeTerm, @ObservationText, @ObservationValue, @ObservationValue2, @ObservationTag, @ObservationCheckSum, @ObservationType);";

                connection.Execute(sql, Observations.Select(o => new
                {
                   o.ObservationID,
                   o.PatientID,
                   o.AppointmentID,
                   o.ObservationDate,
                   o.ObservationCodeTerm,
                   o.ObservationText,
                   o.ObservationValue,
                   o.ObservationValue2,
                   o.ObservationTag,
                   o.ObservationCheckSum,
                   o.ObservationType
                }));

                connection.Close();
            }
        }

        public List<string> GetCheckSums()
        {
            var checksums = new List<string>();
            using var connection = new OleDbConnection(_connectionString);
            {
                connection.Open();

                string sql = "SELECT ObservationCheckSum FROM Observations;";
                checksums = connection.Query<string>(sql).ToList();

                connection.Close();
            }

            return checksums;
        }
    }
}
