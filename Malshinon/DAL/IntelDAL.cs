using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Malshinon.DAL;
using Malshinon.DB;

namespace Malshinon.DAL
{
    public static class IntelDAL
    {
        public static void AddIntelReport(int reporterId, int targetId, string report)
        {
            // Insert the intel report
            DBConnection.Execute(
                $"INSERT INTO intelreports (reporter_id, target_id, intel_text)\r\n" +
                $"VALUES ('{reporterId}', '{targetId}', '{report}')\r\n");

            // Increment reporter's num_reports
            DBConnection.Execute(
                $"UPDATE people SET num_reports = IFNULL(num_reports,0) + 1 WHERE id = {reporterId}");

            // Increment target's num_mentions
            DBConnection.Execute(
                $"UPDATE people SET num_mentions = IFNULL(num_mentions,0) + 1 WHERE id = {targetId}");

            // Check reporter's thresholds
            var reporterStats = DBConnection.Execute(
                $"SELECT num_reports, " +
                $"(SELECT AVG(CHAR_LENGTH(intel_text)) FROM intelreports WHERE reporter_id = {reporterId}) AS avg_length, " +
                $"type FROM people WHERE id = {reporterId}");

            if (reporterStats.Count > 0)
            {
                int numReports = Convert.ToInt32(reporterStats[0]["num_reports"] ?? 0);
                double avgLength = Convert.ToDouble(reporterStats[0]["avg_length"] ?? 0);
                int type = Convert.ToInt32(reporterStats[0]["type"] ?? 0);

                if (numReports >= 10 && avgLength >= 100 && type != (int)Enum.Status.PotentialAgent)
                {
                    DBConnection.Execute(
                        $"UPDATE people SET type = {(int)Enum.Status.PotentialAgent} WHERE id = {reporterId}");
                    Console.WriteLine("Status update: Reporter promoted to Potential Agent.");
                }
            }

            // Check target's thresholds
            var targetStats = DBConnection.Execute(
                $"SELECT num_mentions FROM people WHERE id = {targetId}");

            if (targetStats.Count > 0)
            {
                int numMentions = Convert.ToInt32(targetStats[0]["num_mentions"] ?? 0);
                if (numMentions >= 20)
                {
                    Console.WriteLine("Potential threat alert: Target has been mentioned 20 or more times.");
                }
            }
        }

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

        public static bool IsCapitalized(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || word.Length < 2)
                return false;

            return char.IsUpper(word[0]) && word.Substring(1) == word.Substring(1).ToLower();
        }

        public static string Clean(string word)
        {
            // Remove punctuation from start and end
            return word.Trim('.', ',', ':', ';', '!', '?', '"', '\'');
        }
    }
}
