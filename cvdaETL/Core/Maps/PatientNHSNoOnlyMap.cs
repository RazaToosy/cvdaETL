﻿using CsvHelper.Configuration;
using cvdaETL.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Core.Maps
{
    public sealed class PatientNHSNoOnlyMap : ClassMap<string>
    {
        public PatientNHSNoOnlyMap()
        {
            Map(m => m).Name("NHSNumber");
        }
    }
}