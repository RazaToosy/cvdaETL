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
        [Option('f', "folder", Required = false, HelpText = "Set the folder path.")]
        public string? FolderPath { get; set; }
    }
}
