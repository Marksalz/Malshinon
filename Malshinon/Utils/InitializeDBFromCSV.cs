using Malshinon.DAL;
using Malshinon.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Malshinon.Utils
{
    public static class InitializeDBFromCSV
    {
        public static void ImportReportsFromCsv()
        {
            Console.Write("Enter the path to the CSV file: ");
            string filePath = Console.ReadLine();

            Logger.Log($"Starting import from CSV: {filePath}");

            if (!File.Exists(filePath))
            {
                Logger.Log($"File not found: {filePath}");
                Console.WriteLine("File not found.");
                return;
            }

            int imported = 0, failed = 0;
            using (var reader = new StreamReader(filePath))
            {
                string header = reader.ReadLine(); // skip header
                int lineNumber = 1;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Expecting: reporter_id,reporter_name,target_id,target_name,intel_text,intel_timestamp
                    var parts = SplitCsvLine(line);
                    if (parts.Length < 6)
                    {
                        Logger.Log($"Line {lineNumber}: Invalid format (less than 6 fields). Line skipped.");
                        failed++;
                        continue;
                    }

                    int reporterId;
                    if (!int.TryParse(parts[0], out reporterId) || reporterId <= 0)
                    {
                        Logger.Log($"Line {lineNumber}: Invalid reporter_id '{parts[0]}'. Line skipped.");
                        failed++;
                        continue;
                    }
                    string reporterName = parts[1];

                    
                    int targetId;
                    if (!int.TryParse(parts[2], out targetId) || targetId <= 0)
                    {
                        Logger.Log($"Line {lineNumber}: Invalid target_id '{parts[2]}'. Line skipped.");
                        failed++;
                        continue;
                    }
                    string targetName = parts[3];

                    string report = parts[4];
                    string timestamp = parts[5];

                    try
                    {
                        IntelService.submitReportFromCsv(reporterId, reporterName, targetId, targetName, report, timestamp);
                        imported++;
                        Logger.Log($"Line {lineNumber}: Successfully imported report (reporter_id={reporterId}, target_id={targetId}).");
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        Logger.Log($"Line {lineNumber}: Exception during import: {ex.Message}");
                    }
                }
            }

            Logger.Log($"Import complete. {imported} reports imported, {failed} failed.");
            //Console.WriteLine($"Import complete. {imported} reports imported, {failed} failed.");
        }

        // Simple CSV parser for comma-separated, optionally quoted fields
        private static string[] SplitCsvLine(string line)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '\"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            result.Add(sb.ToString());
            return result.ToArray();
        }
    }
}
