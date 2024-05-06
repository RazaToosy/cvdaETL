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
    public class InteractionsMap : ClassMap<ModelInteraction>
    {
        public InteractionsMap()
        {
            Map(m => m.NHSNumber).Name("NHS Number").TypeConverter<NHSTrimmedConverter>(); ;
            Map(m => m.RecallTeamName).Name("User Details' Full Name");
            Map(m => m.InteractionDate).Name("Date");
            Map(m => m.InteractionCodeTerm).Name("Code Term");
            Map(m => m.InteractionComments).Name("Associated Text");
        }
       
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
