using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using cvdaETL.Core.Models;

namespace cvdaETL.Core.Maps
{
    public class ObservationsInBaseMap : ClassMap<ModelObservationInBase>
    {
        public ObservationsInBaseMap()
        {
            Map(m => m.NHSNumber).Name("NHS Number");
            Map(m => m.Hba1C).Name("HbA1c").TypeConverter(new CustomDecimalConverter()); 
            Map(m => m.Hba1cDate).Name("HbA1c Date").TypeConverter(new CustomDateTimeConverter()); 
            Map(m => m.SystolicBP).Name("Systolic BP").TypeConverter(new CustomDecimalConverter());
            Map(m => m.DiastolicBP).Name("Diastolic BP").TypeConverter(new CustomDecimalConverter());
            Map(m => m.SystolicBPDate).Name("Systolic BP Date").TypeConverter(new CustomDateTimeConverter());
            Map(m => m.DiastolicBPDate).Name("Diastolic BP Date").TypeConverter(new CustomDateTimeConverter());
            Map(m => m.Cholesterol).Name("Cholesterol").TypeConverter(new CustomDecimalConverter());
            Map(m => m.CholesterolDate).Name("Cholesterol Date").TypeConverter(new CustomDateTimeConverter());
        }
    }

    public class CustomDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;//ToString("MM/dd/yyyy");
            }
            return null; // or throw an exception, depending on your needs
        }
    }

    public class CustomDecimalConverter : DecimalConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0m; // Return 0 if the field is blank or null
            }
            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            return 0m; // Return 0 if conversion fails
        }
    }
}
