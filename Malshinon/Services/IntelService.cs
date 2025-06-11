using System;
using System.Collections.Generic;
using System.Linq;
using Malshinon.DAL;

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
            Console.WriteLine("Report submitted successfully!");
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
