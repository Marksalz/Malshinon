using System;
using System.Collections.Generic;
using System.Linq;
using Malshinon.DAL;
using Malshinon.Services;

namespace Malshinon.Services
{
    /// <summary>
    /// Provides high-level services for submitting and processing intelligence reports.
    /// </summary>
    public static class IntelService
    {
        /// <summary>
        /// Submits a new intelligence report and triggers alert generation if needed.
        /// </summary>
        /// <param name="report">The report text.</param>
        /// <param name="reporterCode">The reporter's secret code.</param>
        public static void SubmitReport(string report, string reporterCode)
        {
            string targetName = ExtractNameOfTarget(report);
            if (string.IsNullOrWhiteSpace(targetName))
            {
                Console.WriteLine("No target name found in the report. Please include a target name.");
                return;
            }
            string[] nameSplit = targetName.Split(' ');
            var targetPerson = PeopleDAL.GetPeople(nameSplit[0], nameSplit[1]).FirstOrDefault();
            if (targetPerson == null)
            {
                PeopleDAL.AddPerson(nameSplit[0], nameSplit[1], Enum.Status.Target);
                targetPerson = PeopleDAL.GetPeople(nameSplit[0], nameSplit[1]).FirstOrDefault();
            }
            int reporterId = PeopleService.GetPersonIdBySecretCode(reporterCode);
            int targetId = PeopleService.GetPersonIdBySecretCode(targetPerson["secret_code"].ToString());
            IntelDAL.AddIntelReport(reporterId, targetId, report);
            AlertService.genarateAlertIfNeeded(targetId);
            //Console.WriteLine("Report submitted successfully!");
        }

        /// <summary>
        /// Submits a report from a CSV import, creating reporter/target if needed.
        /// </summary>
        /// <param name="reporterId">Reporter ID.</param>
        /// <param name="reporterName">Reporter name.</param>
        /// <param name="targetId">Target ID.</param>
        /// <param name="targetName">Target name.</param>
        /// <param name="intelText">Report text.</param>
        /// <param name="timestamp">Timestamp of the report.</param>
        public static void submitReportFromCsv(int reporterId, string reporterName, int targetId,
            string targetName, string intelText, string timestamp)
        {
            // split reporterName to get first and last names
            string[] reporterNameSplit = reporterName.Trim().Split(' ');
            if (reporterNameSplit.Length < 2)
            {
                throw new Exception("Invalid reporter name format. Please provide both first and last names.");
            }
            string reporterFirstName = reporterNameSplit[0];
            string reporterLastName = string.Join(" ", reporterNameSplit.Skip(1));

            // split targetName to get first and last names
            string[] targetNameSplit = targetName.Trim().Split(' ');
            if (targetNameSplit.Length < 2)
            {
                throw new Exception("Invalid target name format. Please provide both first and last names.");
            }
            string targetFirstName = targetNameSplit[0];
            string targetLastName = string.Join(" ", targetNameSplit.Skip(1));

            // Check if reporter exists, if not, add them
            var reporterPerson = PeopleDAL.GetPeople(reporterFirstName, reporterLastName).FirstOrDefault();
            if (reporterPerson == null)
            {
                PeopleDAL.AddPerson(reporterFirstName, reporterLastName, Enum.Status.Reporter);
                reporterPerson = PeopleDAL.GetPeople(reporterFirstName, reporterLastName).FirstOrDefault();
            }
            reporterId = PeopleService.GetPersonIdBySecretCode(reporterPerson["secret_code"].ToString());

            // Check if target exists, if not, add them
            var targetPerson = PeopleDAL.GetPeople(targetFirstName, targetLastName).FirstOrDefault();
            if (targetPerson == null)
            {
                PeopleDAL.AddPerson(targetFirstName, targetLastName, Enum.Status.Target);
                targetPerson = PeopleDAL.GetPeople(targetFirstName, targetLastName).FirstOrDefault();
            }
            targetId = PeopleService.GetPersonIdBySecretCode(targetPerson["secret_code"].ToString());
            IntelDAL.AddIntelReportWithTimestamp(reporterId, targetId, intelText, timestamp);
            AlertService.genarateAlertIfNeeded(targetId);
        }

        /// <summary>
        /// Extracts the name of the target from a report string.
        /// </summary>
        /// <param name="input">The report text.</param>
        /// <returns>The extracted name, or null if not found.</returns>
        public static string ExtractNameOfTarget(string input)
        {
            string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length - 1; i++)
            {
                string first = Clean(words[i]);
                string second = Clean(words[i + 1]);

                if (IsCapitalized(first) && IsCapitalized(second))
                    return first + " " + second;
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
