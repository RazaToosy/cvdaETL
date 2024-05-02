using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace cvdaETL
{
    internal class Options
    {
        [Option('f', "CsvPath", Required = true, HelpText = "Set the csv path.")]
        public string? CsvPath { get; set; }

        [Option('d', "InsertDate", Required = true, HelpText = "Set the Insert Date.")]
        public string? InsertDate { get; set; }
    }
}
