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
        [Option('f', "csvlocation", Required = false, HelpText = "Set the csv path.")]
        public string? CsvPath { get; set; }
    }
}
