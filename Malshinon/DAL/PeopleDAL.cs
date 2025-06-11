using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon.DAL
{
    public static class PeopleDAL
    {
        public static void AddPerson(string firstName, string lastName, Enum.Status status = Enum.Status.Reporter)
        {
            DBConnection.Execute(
                $"INSERT INTO people (first_name, last_name, secret_code, type)\r\n" +
                $"VALUES ('{firstName}', '{lastName}', '{GenerateSecretCode(firstName, lastName)}', '{(int)status}')\r\n");
        }

        public static string GenerateSecretCode(string firstName, string lastName)
        {
            // Fix: Replace range operator with substring to ensure compatibility with C# 7.3  
            var code = $"{firstName.Substring(0, 2).ToUpper()}{lastName.Substring(0, 2).ToUpper()}{new Random().Next(100, 999)}";
            return code;
        }

        public static List<Dictionary<string, object>> GetPeople(string firstName = null, string lastName = null)
        {
            var sql = "SELECT * FROM people WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(firstName))
                sql += $" AND first_name LIKE '%{firstName}%'";
            if (!string.IsNullOrWhiteSpace(lastName))
                sql += $" AND last_name LIKE '%{lastName}%'";

            return DBConnection.Execute(sql);
        }

        public static List<Dictionary<string, object>> GetPersonBySecretCode(string secretCode)
        {
            var sql = $"SELECT * FROM people WHERE 1=1 AND secret_code LIKE '{secretCode}'";
            return DBConnection.Execute(sql);
        }

        public static int GetPersonId(string secretCode)
        {
            var person = GetPersonBySecretCode(secretCode);
            if (person != null && person[0].ContainsKey("id"))
            {
                return Convert.ToInt32(person[0]["id"]);
            }
            return -1; // or throw an exception if preferred  
        }

        public static bool IsPersonExists(string secretCode)
        {
            List<Dictionary<string, object>> person = GetPersonBySecretCode(secretCode);
            DBConnection.PrintResult(person);
            return person != null;
        }
    }
}
