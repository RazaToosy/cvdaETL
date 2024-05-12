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
            Map(m => m.NHSNumber).Name("NHSNumber");
            Map(m => m.Hba1C).Name("Hba1c").TypeConverter(new CustomDecimalConverter()); 
            Map(m => m.Hba1cDate).Name("DateOfHba1c").TypeConverter(new CustomDateTimeConverter()); 
            Map(m => m.SystolicBP).Name("SystolicBP").TypeConverter(new CustomDecimalConverter());
            Map(m => m.DiastolicBP).Name("DiastolicBP").TypeConverter(new CustomDecimalConverter());
            Map(m => m.SystolicBPDate).Name("DateOfSystolic").TypeConverter(new CustomDateTimeConverter());
            Map(m => m.DiastolicBPDate).Name("DateOfDiastolic").TypeConverter(new CustomDateTimeConverter());
            Map(m => m.Cholesterol).Name("CholLevel").TypeConverter(new CustomDecimalConverter());
            Map(m => m.CholesterolDate).Name("DateOfCholesterol").TypeConverter(new CustomDateTimeConverter());
        }
    }

    public class CustomDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null; // Return null if the field is blank
            }
            if (DateTime.TryParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
            return base.ConvertFromString(text, row, memberMapData);
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
