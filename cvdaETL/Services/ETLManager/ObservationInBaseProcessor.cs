using cvdaETL.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Enums;
using cvdaETL.Data;
using cvdaETL.Core.Maps;
using cvdaETL.Core.Models;
using cvdaETL.Services.CsvHelper;

namespace cvdaETL.Services.ETLManager
{
    public class ObservationInBaseProcessor
    {
        ResolveDb _dbAccess;

        public ObservationInBaseProcessor(ResolveDb dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public void ImportObservationInBase()
        {
            if (!File.Exists(Path.Combine(Repo.Instance.CsvPath, "Base.csv"))) return;
            var observationsInBase = new CsvHelperManager().ImportFromCsv<ModelObservationInBase, ObservationsInBaseMap>(Path.Combine(Repo.Instance.CsvPath, "Base.csv"));
            
            var checkSums = _dbAccess.ObservationsAccess.GetCheckSums();
            var patients = _dbAccess.PatientAccess.GetNHSNumbers();
            
            var observations = new List<ModelObservation>();

            foreach (var observation in observationsInBase)
            {
                var patientID = patients.FirstOrDefault(x => x.Value == observation.NHSNumber).Key;

                if (patientID == null)  continue;

                //Hba1c
                var hba1cCheckSum = $"{observation.Hba1cDate}{patientID}{ObservationTag.Hba1C}";

                if (!checkSums.Contains(hba1cCheckSum))
                {
                    if (observation.Hba1cDate != null)
                    {
                        observations.Add(new ModelObservation
                        {
                            PatientID = patientID,
                            ObservationCheckSum = $"{observation.Hba1cDate}{patientID}{ObservationTag.Hba1C}",
                            ObservationCodeTerm = "Hba1C",
                            ObservationDate = observation.Hba1cDate.Value,
                            ObservationID = Guid.NewGuid().ToString(),
                            ObservationTag = ObservationTag.Hba1C.ToString(),
                            ObservationValue = observation.Hba1C
                        }) ;
                    }
                }

                //Blood Pressure
                var BPCheckSum = $"{observation.SystolicBPDate}{patientID}{ObservationTag.BloodPressure}";
                if (!checkSums.Contains(BPCheckSum))
                {
                    if (observation.SystolicBPDate != null)
                    {
                        observations.Add(new ModelObservation
                        {
                            PatientID = patientID,
                            ObservationCheckSum = $"{observation.SystolicBPDate}{patientID}{ObservationTag.BloodPressure}",
                            ObservationCodeTerm = "O/E - blood pressure reading",
                            ObservationDate = observation.SystolicBPDate.Value,
                            ObservationID = Guid.NewGuid().ToString(),
                            ObservationTag = ObservationTag.BloodPressure.ToString(),
                            ObservationValue = observation.SystolicBP,
                            ObservationValue2 = observation.DiastolicBP
                        });
                    }
                }

                //Chol
                var CholCheckSum = $"{observation.CholesterolDate}{patientID}{ObservationTag.Cholesterol}";
                if (!checkSums.Contains(CholCheckSum))
                {
                    if (observation.CholesterolDate != null)
                    {
                        observations.Add(new ModelObservation
                        {
                            PatientID = patientID,
                            ObservationCheckSum =
                                $"{observation.CholesterolDate}{patientID}{ObservationTag.Cholesterol}",
                            ObservationCodeTerm = "Cholesterol",
                            ObservationDate = observation.CholesterolDate.Value,
                            ObservationID = Guid.NewGuid().ToString(),
                            ObservationTag = ObservationTag.Cholesterol.ToString(),
                            ObservationValue = observation.Cholesterol
                        });
                    }
                }
            }
            
            _dbAccess.ObservationsAccess.InsertObservations(observations);

        }
    }
}
