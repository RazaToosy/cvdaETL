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
using Serilog;

namespace cvdaETL.Services.ETLManager
{
    public class PatientProcessor
    {

        public void ImportPatients()
        {
            // Import the patients from the CSV file
            var patients = new CsvHelperManager().ImportFromCsv<ModelPatient, PatientMap>(Path.Combine(Repo.Instance.CsvPath, "Base.csv"));
            var excludedFromRecall = new CsvHelperManager().ImportFromCsv<string, PatientNHSNoOnlyMap>(Path.Combine(Repo.Instance.CsvPath, "Excluded.csv"));
            var activePatients = new CsvHelperManager().ImportFromCsv<string, PatientNHSNoOnlyMap>(Path.Combine(Repo.Instance.CsvPath, "Recall.csv"));
            var existingNHSNumbers = new PatientDbAccess().GetNHSNumbers();

            // Create lists to hold patients for update and insert
            List<ModelPatient> patientsForUpdate = new List<ModelPatient>();
            List<ModelPatient> patientsForInsert = new List<ModelPatient>();

            patients.ForEach(patient =>
            {
                
                 // Set the patient's current state based on the conditions
                patient.CurrentState = excludedFromRecall.Any(p => p == patient.NHSNumber) ? RegisterState.RecallExcluded.ToString() :
                                       activePatients.Any(p => p == patient.NHSNumber) ? RegisterState.Active.ToString() :
                                       RegisterState.Removed.ToString();
                
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
                    new RegiserDbAccess().InsertRegister(register);
                }

                new PatientDbAccess().UpdatePatients(patientsForUpdate);
                new PatientDbAccess().InsertPatients(patientsForInsert);

            });


            Log.Information("Imported {0} patients from the CSV file into DB.", patients.Count);
            Log.Information("Imported {0} new patients finto DB.", patientsForInsert.Count);

            Repo.Instance.CvdaPatients = patients;

        }
    }
}
