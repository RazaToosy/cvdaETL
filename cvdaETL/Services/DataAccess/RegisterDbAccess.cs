﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;

namespace cvdaETL.Services.DataAccess
{
    public class RegisterDbAccess : IDbRegisterAccess
    {
        private string _connectionString;
        public RegisterDbAccess()
        {
            _connectionString = Repo.Instance.ConnectionString;
        }

        public void InsertRegister(List<ModelRegister> register)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var sql = "INSERT INTO Registers (RegisterID, PatientID, RegisterDate, RegisterState) VALUES (@RegisterID, @PatientID, @RegisterDate, @RegisterState)";
                connection.Execute(sql, register);
            }
        }

    }
}