using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using Dapper;

namespace cvdaETL.Services.DataAccess
{
    public class RegisterAccdbAccess : IDbRegisterAccess
    {

        private string _connectionString;
        public RegisterAccdbAccess()
        {
            _connectionString = Repo.Instance.ConnectionString;
        }
        
        public void InsertRegister(List<ModelRegister> register)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                
                
                var sql = "INSERT INTO Registers (RegisterID, PatientID, RegisterDate, RegisterState) VALUES (?, ?, ?, ?)";

                foreach (var reg in register)
                {
                    // Using Dapper's Execute method directly with an anonymous type to map parameters in the correct order
                    connection.Execute(sql, new
                    {
                        RegisterID = reg.RegisterID,
                        PatientID = reg.PatientID,
                        RegisterDate = DateTime.ParseExact(reg.RegisterDate.ToString("MM/dd/yyyy"), "MM/dd/yyyy", CultureInfo.InvariantCulture),
                        RegisterState = reg.RegisterState
                    });
                }

                connection.Close();
            }
        }
    }
}
