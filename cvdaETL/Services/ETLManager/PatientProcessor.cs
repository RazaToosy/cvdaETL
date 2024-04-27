using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Enums;
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

        public void ImportPatients()
        {
            // Import the patients from the CSV file
            var patients = new CsvHelperManager().ImportFromCsv<ModelPatient, PatientMap>(Path.Combine(Repo.Instance.CsvPath, "Base.csv"));
            var excludedFromRecall = new CsvHelperManager().ImportFromCsv<ModelPatientNHSNoOnly, PatientNHSNoOnlyMap>(Path.Combine(Repo.Instance.CsvPath, "Excluded.csv"));
            var activePatients = new CsvHelperManager().ImportFromCsv<ModelPatientNHSNoOnly, PatientNHSNoOnlyMap>(Path.Combine(Repo.Instance.CsvPath, "Recall.csv"));
            var existingNHSNumbers = new PatientDbAccess().GetNHSNumbers();

            // Create lists to hold patients for update and insert
            List<ModelPatient> patientsForUpdate = new List<ModelPatient>();
            List<ModelPatient> patientsForInsert = new List<ModelPatient>();
            List<ModelRegister> patientsForRegister = new List<ModelRegister>();

            patients.ForEach(patient =>
            {
                
                 // Set the patient's current state based on the conditions
                patient.CurrentState = activePatients.Any(p => p.NHSNumber == patient.NHSNumber) ? RegisterState.Active.ToString() :
                    excludedFromRecall.Any(p => p.NHSNumber == patient.NHSNumber) ? RegisterState.RecallExclusions.ToString() :
                    RegisterState.ConditionExclusions.ToString();

                // Check if the patient is in the existing Patient Table
                if (existingNHSNumbers.ContainsKey(patient.NHSNumber))
                {
                    // Add the patient to the list for update
                    patientsForUpdate.Add(patient);
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
                        RegisterDate = DateTime.Now,
                        RegisterState = RegisterState.New.ToString()
                    };
                    patientsForRegister.Add(register);
                    
                    if (patient.CurrentState == RegisterState.Active.ToString()) //Also add log of when active patients joined
                    {
                        var active = new ModelRegister
                        {
                            RegisterID = Guid.NewGuid().ToString(),
                            PatientID = patient.PatientID,
                            RegisterDate = DateTime.Now,
                            RegisterState = RegisterState.Active.ToString()
                        };
                        patientsForRegister.Add(active);
                    }
                    
                }

            });
            Console.WriteLine("Patients for update: " + patientsForUpdate.Count);
            Console.WriteLine("Patients for insert: " + patientsForInsert.Count);
            Console.WriteLine("Accessing Patient Table and Updating...");
            new PatientDbAccess().UpdatePatients(patientsForUpdate);
            new PatientDbAccess().InsertPatients(patientsForInsert);
            Console.WriteLine("Accessing Register Table and Updating...");
            new RegiserDbAccess().InsertRegister(patientsForRegister);

            Log.Information("Imported {0} patients from the CSV file into DB.", patients.Count);
            Log.Information("Imported {0} new patients finto DB.", patientsForInsert.Count);

            Repo.Instance.CvdaPatients = patients;

        }
    }
}
