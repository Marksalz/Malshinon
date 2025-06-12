using System;
using System.Collections.Generic;
using Malshinon.DB;
using Utils;

namespace Malshinon.DAL
{
    public static class PeopleDAL
    {
        public static void AddPerson(string firstName, string lastName, Enum.Status status = Enum.Status.Reporter)
        {
            DBConnection.Execute(
                $"INSERT INTO people (first_name, last_name, secret_code, type)\r\n" +
                $"VALUES ('{firstName}', '{lastName}', '{GenerateSecretCode(firstName, lastName)}', '{(int)status}')\r\n");
            Logger.Log($"New person created: {firstName} {lastName} (Status: {status})");
        }

        public static string GenerateSecretCode(string firstName, string lastName)
        {
            var code = $"{firstName.Substring(0, 2).ToUpper()}{lastName.Substring(0, 2).ToUpper()}{new Random().Next(100, 999)}";
            return code;
        }

        public static List<Dictionary<string, object>> GetPeople(string firstName = null, string lastName = null)
        {
            var sql = "SELECT * FROM people WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(firstName))
                sql += $" AND first_name = '{firstName}'";
            if (!string.IsNullOrWhiteSpace(lastName))
                sql += $" AND last_name = '{lastName}'";
            return DBConnection.Execute(sql);
        }

        public static List<Dictionary<string, object>> GetPersonBySecretCode(string secretCode)
        {
            var sql = $"SELECT * FROM people WHERE secret_code = '{secretCode}'";
            return DBConnection.Execute(sql);
        }

        public static List<Dictionary<string, object>> GetPersonById(int Id)
        {
            var sql = $"SELECT * FROM people WHERE id = '{Id}'";
            return DBConnection.Execute(sql);
        }

        public static int GetPersonId(string secretCode)
        {
            var person = GetPersonBySecretCode(secretCode);
            if (person != null && person.Count > 0 && person[0].ContainsKey("id"))
            {
                return Convert.ToInt32(person[0]["id"]);
            }
            return -1;
        }

        public static bool IsPersonExists(string secretCode)
        {
            List<Dictionary<string, object>> person = GetPersonBySecretCode(secretCode);
            return person != null && person.Count > 0;
        }

        // Returns all people with type = PotentialAgent
        public static List<Dictionary<string, object>> GetPotentialRecruits()
        {
            return Malshinon.DB.DBConnection.Execute(
                $"SELECT id, first_name, last_name FROM people WHERE type = {(int)Enum.Status.PotentialAgent}");
        }

        // Returns all targets with num_mentions >= 20
        public static List<Dictionary<string, object>> GetDangerousTargets()
        {
            return Malshinon.DB.DBConnection.Execute(
                $"SELECT id, first_name, last_name, num_mentions FROM people WHERE num_mentions >= 20");
        }
    }
}