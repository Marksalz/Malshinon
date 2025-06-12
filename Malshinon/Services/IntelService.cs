using System;
using System.Collections.Generic;
using System.Linq;
using Malshinon.DAL;
using Malshinon.Services;

namespace Malshinon.Services
{
    public static class IntelService
    {
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
