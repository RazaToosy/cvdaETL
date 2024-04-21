using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Enums;
using cvdaETL.Data;
using cvdaETL.Services.CsvHelper;
using Serilog;

namespace cvdaETL.Services.ETLManager
{
    public class PatientProcessor
    {

        private void ImportPatients()
        {
            // Import the patients from the CSV file
            var patients = CsvHelperManager.ReadCsvFile(Repo.Instance.CsvPath);
            // Log.Information("Imported {0} patients from the CSV file.", patients.Count);
            
            // Need to work out the state of the patients from Excluded and RecallExcluded
            // Work out additions and removals
            // populate Regsiter Table with this informaton
            // Insert New Patients into the Patients table
            // Update Patients where the state has changed or update all of them?
            
        }
    }
}
