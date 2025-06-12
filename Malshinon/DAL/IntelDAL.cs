using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malshinon.DAL;
using Malshinon.DB;
using Utils;

namespace Malshinon.DAL
{
    /// <summary>
    /// Provides data access methods for handling intelligence reports and related operations.
    /// </summary>
    public static class IntelDAL
    {
        /// <summary>
        /// Adds a new intelligence report to the database and updates reporter/target statistics.
        /// </summary>
        /// <param name="reporterId">The ID of the person submitting the report.</param>
        /// <param name="targetId">The ID of the target person.</param>
        /// <param name="report">The text of the intelligence report.</param>
        public static void AddIntelReport(int reporterId, int targetId, string report)
        {
            // Insert the intel report
            DBConnection.Execute(
                $"INSERT INTO intelreports (reporter_id, target_id, intel_text)\r\n" +
                $"VALUES ('{reporterId}', '{targetId}', '{report}')\r\n");

            Logger.Log($"Report submitted by reporter ID {reporterId} on target ID {targetId}.");

            // Increment reporter's num_reports
            DBConnection.Execute(
                $"UPDATE people SET num_reports = IFNULL(num_reports,0) + 1 WHERE id = {reporterId}");

            // Increment target's num_mentions
            DBConnection.Execute(
                $"UPDATE people SET num_mentions = IFNULL(num_mentions,0) + 1 WHERE id = {targetId}");

            // Check reporter's thresholds and update status if necessary
            CheckReporterThresholds(reporterId);
        }

        /// <summary>
        /// Adds a new intelligence report with a specific timestamp (used for CSV import).
        /// </summary>
        /// <param name="reporterId">The ID of the reporter.</param>
        /// <param name="targetId">The ID of the target.</param>
        /// <param name="report">The report text.</param>
        /// <param name="timestamp">The timestamp of the report.</param>
        public static void AddIntelReportWithTimestamp(int reporterId, int targetId, string report, string timestamp)
        {
            DBConnection.Execute(
                $"INSERT INTO intelreports (reporter_id, target_id, intel_text, intel_timestamp)\r\n" +
                $"VALUES ('{reporterId}', '{targetId}', '{report}', '{timestamp}')\r\n");

            Logger.Log($"[CSV Import] Report submitted by reporter ID {reporterId} on target ID {targetId} at {timestamp}.");

            DBConnection.Execute(
                $"UPDATE people SET num_reports = IFNULL(num_reports,0) + 1 WHERE id = {reporterId}");

            DBConnection.Execute(
                $"UPDATE people SET num_mentions = IFNULL(num_mentions,0) + 1 WHERE id = {targetId}");

            CheckReporterThresholds(reporterId);
        }

        /// <summary>
        /// Checks if a reporter meets the criteria for promotion and updates their status if needed.
        /// </summary>
        /// <param name="reporterId">The ID of the reporter to check.</param>
        public static void CheckReporterThresholds(int reporterId)
        {
            // Get reporter's number of reports from intelreports table
            var reporterStats = DBConnection.Execute(
                $"SELECT " +
                $"(SELECT COUNT(*) FROM intelreports WHERE reporter_id = {reporterId}) AS num_reports, " +
                $"(SELECT AVG(CHAR_LENGTH(intel_text)) FROM intelreports WHERE reporter_id = {reporterId}) AS avg_length, " +
                $"type FROM people WHERE id = {reporterId}");

            if (reporterStats.Count > 0)
            {
                int numReports = Convert.ToInt32(reporterStats[0]["num_reports"] ?? 0);
                double avgLength = Convert.ToDouble(reporterStats[0]["avg_length"] ?? 0);
                int type = 0;
                if (reporterStats[0]["type"] != null)
                {
                    var typeStr = reporterStats[0]["type"].ToString().Trim().ToLower();
                    switch (typeStr)
                    {
                        case "reporter":
                            type = (int)Enum.Status.Reporter;
                            break;
                        case "target":
                            type = (int)Enum.Status.Target;
                            break;
                        case "both":
                            type = (int)Enum.Status.Both;
                            break;
                        case "potential_agent":
                            type = (int)Enum.Status.PotentialAgent;
                            break;
                    }
                }

                if (numReports >= 10 && avgLength >= 100 && type != (int)Enum.Status.PotentialAgent)
                {
                    DBConnection.Execute(
                        $"UPDATE people SET type = {(int)Enum.Status.PotentialAgent} WHERE id = {reporterId}");
                    string msg = "Status update: Reporter promoted to Potential Agent.";
                    Console.WriteLine(msg);
                    Logger.Log($"Reporter ID {reporterId} promoted to Potential Agent.");
                }
            }
        }

        /// <summary>
        /// Attempts to extract a target's name from a report string.
        /// </summary>
        /// <param name="report">The report text.</param>
        /// <returns>The extracted name, or null if not found.</returns>
        public static string ExtractNameOfTarget(string report)
        {
            string[] words = report.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length - 1; i++)
            {
                string first = Clean(words[i]);
                string second = Clean(words[i + 1]);

                // Check if both are capitalized
                if (IsCapitalized(first) && IsCapitalized(second))
                {
                    // Ensure neither is the first word of a sentence
                    bool firstIsSentenceStart = (i == 0) || (words[i - 1].EndsWith(".") || words[i - 1].EndsWith("!") || words[i - 1].EndsWith("?"));
                    bool secondIsSentenceStart = (words[i].EndsWith(".") || words[i].EndsWith("!") || words[i].EndsWith("?"));

                    if (!firstIsSentenceStart && !secondIsSentenceStart)
                        return first + " " + second;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if a word is capitalized (first letter uppercase, rest lowercase).
        /// </summary>
        /// <param name="word">The word to check.</param>
        /// <returns>True if capitalized, otherwise false.</returns>
        public static bool IsCapitalized(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || word.Length < 2)
                return false;

            return char.IsUpper(word[0]) && word.Substring(1) == word.Substring(1).ToLower();
        }

        /// <summary>
        /// Removes punctuation from the start and end of a word.
        /// </summary>
        /// <param name="word">The word to clean.</param>
        /// <returns>The cleaned word.</returns>
        public static string Clean(string word)
        {
            // Remove punctuation from start and end
            return word.Trim('.', ',', ':', ';', '!', '?', '"', '\'');
        }
    }
}
