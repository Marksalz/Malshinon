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
        //public static void SubmitReport(string report, string reporterCode)
        //{
        //    string targetName = ExtractNameOfTarget(report);
        //    if (string.IsNullOrWhiteSpace(targetName))
        //    {
        //        Console.WriteLine("No target name found in the report. Please include a target name.");
        //        return;
        //    }
        //    string[] nameSplit = targetName.Split(' ');
        //    Dictionary<string, object> targetPerson = PeopleDAL.GetPeople(nameSplit[0], nameSplit[1]).FirstOrDefault();
        //    if (targetPerson == null)
        //    {
        //        Console.WriteLine("Target person not found in the database. Adding to database.");
        //        PeopleDAL.AddPerson(nameSplit[0], nameSplit[1], Enum.Status.Target);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Target person {targetPerson["first_name"]} {targetPerson["last_name"]} already exists in the database.");
        //    }
        //    int reporterId = PeopleDAL.GetPersonId(reporterCode);
        //    int targetId;
        //    if (targetPerson == null)
        //    {
        //        // The person was just added, so retrieve by name again
        //        Dictionary<string, object> addedPerson = PeopleDAL.GetPeople(nameSplit[0], nameSplit[1]).FirstOrDefault();
        //        if (addedPerson == null)
        //        {
        //            Console.WriteLine("Failed to retrieve the newly added target person.");
        //            return;
        //        }
        //        targetId = PeopleDAL.GetPersonId(addedPerson["secret_code"].ToString());
        //    }
        //    else
        //    {
        //        targetId = PeopleDAL.GetPersonId(targetPerson["secret_code"].ToString());
        //    }
        //    AddIntelReport(reporterId, targetId, report);
        //    Console.WriteLine($"Report submited succsefully! ");
        //}

        public static void AddIntelReport(int reporterId, int targetId, string report)
        {
            DBConnection.Execute(
                $"INSERT INTO intelreports (reporter_id, target_id, intel_text)\r\n" +
                $"VALUES ('{reporterId}', '{targetId}', '{report}')\r\n");
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
