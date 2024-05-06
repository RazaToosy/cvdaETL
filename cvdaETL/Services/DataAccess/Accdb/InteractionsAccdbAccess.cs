using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using Dapper;
using Microsoft.VisualBasic;

namespace cvdaETL.Services.DataAccess.Accdb
{
    public class InteractionsAccdbAccess : IDbInteractionsAccess
    {
        private readonly string _connectionString;
        public InteractionsAccdbAccess()
        {
            _connectionString = Repo.Instance.ConnectionString;
        }
        
        public void InsertInteractions(List<ModelInteraction> Interactions)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                
                var sql = "INSERT INTO Interactions (InteractionID, PatientID, RecallTeamID, InteractionDate, InteractionCodeTerm, InteractionComments, InteractionType) VALUES (@InteractionID, @PatientID, @RecallTeamID, @InteractionDate, @InteractionCodeTerm, @InteractionComments, @InteractionType);";

                connection.Execute(sql, Interactions.Select(i => new
                {
                    i.InteractionID,
                    i.PatientID,
                    i.RecallTeamID,
                    i.InteractionDate,
                    i.InteractionCodeTerm,
                    i.InteractionComments,
                    i.InteractionType
                }));
                
               connection.Close();
            }
        }

        public List<ModelInteraction> GetAllInteractions()
        {
            var interactions = new List<ModelInteraction>();
            using var connection = new OleDbConnection(_connectionString);
            {
                connection.Open();

                string sql = "SELECT InteractionID, PatientID, RecallTeamID, InteractionDate, InteractionCodeTerm, InteractionComments, InteractionType FROM Interactions;";
                interactions = connection.Query<ModelInteraction>(sql).ToList();

                connection.Close();
            }

            return interactions;
        }

        public void InsertRecallTeam(ModelRecallTeam RecallTeam)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                var sql =
                    "INSERT INTO RecallTeam (RecallTeamID, RecallTeamName) VALUES (@RecallTeamID, @RecallTeamName);";

                connection.Execute(sql, new
                {
                    RecallTeam.RecallTeamID,
                    RecallTeam.RecallTeamName
                });

            }
        }

        public Dictionary<string, string> GetCurrentRecallTeam()
        {
            Dictionary<string, string> recallTeamWithIDs = new Dictionary<string, string>();
            using var connection = new OleDbConnection(_connectionString);
            {
                connection.Open();

                string sql = "SELECT RecallTeamID, RecallTeamName FROM RecallTeam;";
                recallTeamWithIDs = connection.Query<(string, string)>(sql)
                    .ToDictionary(row => row.Item1, row => row.Item2);

                connection.Close();
            }

            return recallTeamWithIDs;
        }

        public bool CheckIfDateExists(DateTime Date)
        {
            int count = 0;
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                // SQL query to check if the date exists in the InsertDate field
                string query = "SELECT COUNT(*) FROM Interactions WHERE InteractionDate = @InteractionDate;";

                // Execute the query using Dapper
                count = connection.QuerySingle<int>(query, new { InteractionDate = Date });

            }
            return count >  0;
        }
    }
}
