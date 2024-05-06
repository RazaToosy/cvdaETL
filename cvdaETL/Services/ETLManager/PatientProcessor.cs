using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Enums;
using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Maps;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using cvdaETL.Services.CsvHelper;
using cvdaETL.Services.DataAccess;
using Microsoft.Win32;
using Serilog;

namespace cvdaETL.Services.ETLManager
{
    public class PatientProcessor
    {
        ResolveDb _dbAccess;

        public PatientProcessor(ResolveDb dbAccess)
        {
            _dbAccess = dbAccess;
        }
        public void ImportPatients()
        {
            // Import the patients from the CSV file
            var patients = new CsvHelperManager().ImportFromCsv<ModelPatient, PatientMap>(Path.Combine(Repo.Instance.CsvPath, "Base.csv"));
            var excludedFromRecall = new CsvHelperManager().ImportFromCsv<ModelPatientNHSNoOnly, PatientNHSNoOnlyMap>(Path.Combine(Repo.Instance.CsvPath, "Excluded.csv"));
            var activePatients = new CsvHelperManager().ImportFromCsv<ModelPatientNHSNoOnly, PatientNHSNoOnlyMap>(Path.Combine(Repo.Instance.CsvPath, "Recall.csv"));

            var existingNHSNumbers = _dbAccess.PatientAccess.GetNHSNumbers();

            // Create lists to hold patients for update and insert
            List<ModelPatient> patientsForUpdate = new List<ModelPatient>();
            List<ModelPatient> patientsForInsert = new List<ModelPatient>();
            List<ModelRegister> patientsForRegister = new List<ModelRegister>();

            Dictionary<string, string> cvdaTargets = new Dictionary<string, string>();

            patients.ForEach(patient =>
            {
                // Set the patient's current state based on the conditions
                patient.CurrentState = activePatients.Any(p => p.NHSNumber == patient.NHSNumber) ? RegisterState.Active.ToString() :
                    excludedFromRecall.Any(p => p.NHSNumber == patient.NHSNumber) ? RegisterState.RecallExclusions.ToString() :
                    RegisterState.ConditionExclusions.ToString();

                // Check if the patient is in the existing Patient Table
                if (existingNHSNumbers.ContainsValue(patient.NHSNumber))
                {
                    patient.PatientID = existingNHSNumbers.FirstOrDefault(x => x.Value == patient.NHSNumber).Key;
                    // Add the patient to the list for update
                    //patientsForUpdate.Add(patient);
                }
                else
                {
                    // Add the patient to the list for insert
                    patient.PatientID = Guid.NewGuid().ToString();
                    patientsForInsert.Add(patient);
                    var register = new ModelRegister
                    {
                        RegisterID = Guid.NewGuid().ToString(),
                        PatientID = patient.PatientID,
                        RegisterDate = Repo.Instance.InsertDate,
                        RegisterState = RegisterState.New.ToString()
                    };
                    patientsForRegister.Add(register);
                    
                    if (patient.CurrentState == RegisterState.Active.ToString()) //Also add log of when active patients joined
                    {
                        var active = new ModelRegister
                        {
                            RegisterID = Guid.NewGuid().ToString(),
                            PatientID = patient.PatientID,
                            RegisterDate = Repo.Instance.InsertDate,
                            RegisterState = RegisterState.Active.ToString()
                        };
                        patientsForRegister.Add(active);
                    }
                    
                }

                if (!cvdaTargets.ContainsKey(patient.PatientID))
                    cvdaTargets.Add(patient.PatientID, patient.CVDATargets);
            });
            Console.WriteLine("Patients for update: " + patientsForUpdate.Count);
            Console.WriteLine("Accessing Patient Table and Updating...");
            _dbAccess.PatientAccess.UpdatePatients(patientsForUpdate);

            Console.WriteLine("Patients for insert: " + patientsForInsert.Count);
            Console.WriteLine("Accessing Patient Table and Inserting...");
            _dbAccess.PatientAccess.InsertPatients(patientsForInsert);       
            
            Console.WriteLine("Inserting Registers...");
            _dbAccess.RegisterAccess.InsertRegister(patientsForRegister);
            
            Console.WriteLine("Inserting Conditions and Targets...");
            _dbAccess.ConditionsAndTargetsAccess.InsertConditionsAndTargets(cvdaTargets);

            Log.Information("Imported {0} patients from the CSV file into DB.", patients.Count);
            Log.Information("Imported {0} new patients into DB.", patientsForInsert.Count);

            Repo.Instance.CvdaPatients = patients;
            Repo.Instance.PatientIDsNHSNumber = _dbAccess.PatientAccess.GetNHSNumbers();

        }
    }
}
