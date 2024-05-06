using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using cvdaETL.Core.Models;

namespace cvdaETL.Core.Maps
{
    public class AppointmentStaffMap : ClassMap<ModelAppointment>
    {
        public AppointmentStaffMap()
        {
            Map(m => m.NHSNumber).Name("NHS Number").TypeConverter<NHSTrimmedConverter>();
            Map(m => m.StaffName).Name("Session Holder's Full Name");
            Map(m => m.StaffRole).Name("Session Holder's User Type");
            Map(m => m.AppointmentDateTime).Name("Appointment Date");
            Map(m => m.AppointmentTime).Name("Appointment Time");
            Map(m => m.AppointmentPlannedTime).Name("Planned Appointment Duration");
            Map(m => m.AppointmentActualTime).Name("Consultation Time").Convert(args =>
            {
                var field = args.Row.GetField("Consultation Time");
                return string.IsNullOrEmpty(field) ? 0 : int.Parse(field);
            });
            Map(m => m.AppointmentLocation).Name("Location's Name");
            Map(m => m.AppointmentPostCode).Name("Location's Postcode");
            Map(m => m.AppointmentType).Name("Slot Type");
            Map(m => m.AppointmentOutcome).Name("Current Slot Status");
        }
        
    }

    public class NHSTrimmedConverter : ITypeConverter
    {
        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            // Remove spaces
            return text?.Replace(" ", string.Empty);
        }

        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value?.ToString();
        }
    }
}
