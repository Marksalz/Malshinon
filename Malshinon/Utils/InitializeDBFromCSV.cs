using Malshinon.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon.Utils
{
    public static class InitializeDBFromCSV
    {
        public static void ImportReportsFromCsv()
        {
            Console.Write("Enter the path to the CSV file: ");
            string filePath = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }

            int imported = 0, failed = 0;
            using (var reader = new StreamReader(filePath))
            {
                string header = reader.ReadLine(); // skip header
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Expecting: reporter_id,target_id,intel_text,intel_timestamp
                    var parts = SplitCsvLine(line);
                    if (parts.Length < 4)
                    {
                        failed++;
                        continue;
                    }

                    int reporterId, targetId;
                    string report = parts[2];
                    string timestamp = parts[3];

                    if (!int.TryParse(parts[0], out reporterId) || !int.TryParse(parts[1], out targetId))
                    {
                        failed++;
                        continue;
                    }

                    try
                    {
                        IntelDAL.AddIntelReportWithTimestamp(reporterId, targetId, report, timestamp);
                        imported++;
                    }
                    catch
                    {
                        failed++;
                    }
                }
            }

            Console.WriteLine($"Import complete. {imported} reports imported, {failed} failed.");
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
