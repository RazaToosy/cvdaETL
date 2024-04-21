using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using cvdaETL.Core.Maps;
using cvdaETL.Core.Models;

namespace cvdaETL.Services.CsvHelper
{
    internal class CsvHelperManager
    {
        // where T is the Model and TMap is the ClassMap
        // Example: ImportFromCsv<ModelPatient, PatientMap>(filePath);
        public List<T> ImportFromCsv<T, TMap>(string filePath) where TMap : ClassMap<T>
        {
            List<T> records;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreBlankLines = true, // Explicitly set to skip blank lines
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<TMap>();
                records = csv.GetRecords<T>().ToList();
            }

            return records;
        }
    }
}
