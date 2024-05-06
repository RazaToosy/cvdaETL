using cvdaETL.Core.Interfaces;
using cvdaETL.Core.Maps;
using cvdaETL.Core.Models;
using cvdaETL.Data;
using cvdaETL.Services.CsvHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Services.ETLManager
{
    public class ObservationsProcessor
    {

        ResolveDb _dbAccess;
        
        
        public ObservationsProcessor(ResolveDb dbAccess)
        {
            _dbAccess = dbAccess;
        }
        
        public void ImportObservations()
        {
            var ObsCheckSums = _dbAccess.ObservationsAccess.GetCheckSums();
            var observations = new CsvImportObservations(ObsCheckSums).Import(Path.Combine(Repo.Instance.CsvPath, "Observations.csv"));
            _dbAccess.ObservationsAccess.InsertObservations(observations);
            Repo.Instance.CvdaObservations = observations;

        }
    }
}
