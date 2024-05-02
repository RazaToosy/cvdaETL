﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Data;
using cvdaETL.Services.DataAccess;

namespace cvdaETL.Core.Interfaces
{
    public class ResolveDb
    {

        IDbPatientAccess _patientAccess;
        IDbRegisterAccess _registerAccess;
        IDbConditionsAndTargetsAccess _conditionsAndTargetsAccess;

        public ResolveDb(string ConfigurationString)
        {
            if (ConfigurationString.Contains("accdb"))
            {
                _patientAccess = new PatientAccdbAccess();
                _registerAccess = new RegisterAccdbAccess();
                _conditionsAndTargetsAccess = new ConditionsAndTargetsAccdbAccess();
                Repo.Instance.InsertDate = DateTime.ParseExact(Repo.Instance.InsertDate.ToString("MM/dd/yyyy"), "MM/dd/yyyy",
                    CultureInfo.InvariantCulture); //converts date to MM/dd/yyyy format for Access Insertion
            }
            else
            {
                _patientAccess = new PatientDbAccess();
                _registerAccess = new RegisterDbAccess();
            }
        }

        public IDbPatientAccess PatientAccess
        {
            get => _patientAccess;
            set => _patientAccess = value;
        }

        public IDbRegisterAccess RegisterAccess
        {
            get => _registerAccess;
            set => _registerAccess = value;
        }

        public IDbConditionsAndTargetsAccess ConditionsAndTargetsAccess
        {
            get => _conditionsAndTargetsAccess;
            set => _conditionsAndTargetsAccess = value;
        }
    }
}