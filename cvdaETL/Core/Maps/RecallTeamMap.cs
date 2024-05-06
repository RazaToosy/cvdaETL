using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using cvdaETL.Core.Models;

namespace cvdaETL.Core.Maps
{
    public class RecallTeamMap : ClassMap<ModelRecallTeam>
    {
        public RecallTeamMap()
        {
            Map(m=>m.RecallTeamName).Name("User Details' Full Name");
        }
    }
}
