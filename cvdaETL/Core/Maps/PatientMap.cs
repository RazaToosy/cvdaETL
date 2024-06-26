using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cvdaETL.Core.Models;
using CsvHelper.TypeConversion;
using CsvHelper;
using System.Globalization;

namespace cvdaETL.Core.Maps
{
    public sealed class PatientMap : ClassMap<ModelPatient>
    {
        public PatientMap()
        {
            //Map(m => m.PatientID).Name("PatientID");
            Map(m => m.EmisNo).Name("EMIS Number");
            Map(m => m.ODSCode).Name("ODS Code");
            Map(m => m.NHSNumber).Name("NHS Number");
            Map(m => m.RiskScore).Name("Public Health Score").TypeConverter<IntConverter>();
            Map(m => m.HouseBound).Name("Is Housebound");
            Map(m => m.InHome).Name("Is Housebound");
            Map(m => m.PCN).Name("PCN Name");
            Map(m => m.SurgeryName).Name("Surgery Name");
            //Map(m => m.UsualGP).Name("UsualGP");
            Map(m => m.Surname).Name("Surname");
            Map(m => m.FirstNames).Name("First Names");
            Map(m => m.Title).Name("Title");
            Map(m => m.Sex).Name("Sex");
            Map(m => m.DateOfBirth).Name("Date of Birth").TypeConverter<CustomDateConverter>();
            Map(m => m.Age).Name("Age");
            Map(m => m.HouseNameFlat).Name("House Name/Flat");
            Map(m => m.Street).Name("Street");
            Map(m => m.Village).Name("Village");
            Map(m => m.Town).Name("Town");
            Map(m => m.County).Name("County");
            Map(m => m.PostCode).Name("Postcode");
            Map(m => m.HomeTelephone).Name("Home Telephone");
            Map(m => m.Mobile).Name("Mobile");
            Map(m => m.WorkTelephone).Name("Work Telephone");
            Map(m => m.Email).Name("Email");
            Map(m => m.DeprivationDecile).Name("Deprivation Index").TypeConverter<IntConverter>(); ;
            Map(m => m.HealthDecile).Name("Deprivation Index").TypeConverter<IntConverter>(); ;
            Map(m => m.Ethnicity).Name("Ethnicity (BAME)");
            Map(m => m.PHMData).Name("PHM Data");
            Map(m => m.CVDATargets).Name("MetricShortName");
            Map(m => m.CVDATargets).Name("MetricShortNames");
        }
    }

    public class IntConverter : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0; // or any default value you prefer
            }

            if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
            {
                return (int)doubleValue;
            }
            
            return 0;
        }
    }

    public class CustomDateConverter : DefaultTypeConverter
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
}
