using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Utilities
{ 
public static class CSVUtilities
    {

        public static void Filter(string FullPath)
        {
            // Read all lines of the file
            var lines = File.ReadAllLines(FullPath).ToList();

            // Find the index of the last line that is considered "blank" because it contains only commas
            int lastBlankLineIndex = lines.FindLastIndex(line => line.Trim().All(ch => ch == ','));

            // If a blank line is found and it's not the last line of the file
            if (lastBlankLineIndex != -1 && lastBlankLineIndex == lines.Count - 1)
            {
                // Keep only the lines above the last blank line
                var filteredLines = lines.Take(lastBlankLineIndex).ToList();

                // Overwrite the original file with the filtered content
                File.WriteAllLines(FullPath, filteredLines);
            }
        }
    }
}
