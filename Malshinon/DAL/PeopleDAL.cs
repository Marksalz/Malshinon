using System;
using System.Collections.Generic;
using Malshinon.DB;
using Utils;

namespace Malshinon.DAL
{
    /// <summary>
    /// Provides data access methods for managing people in the system.
    /// </summary>
    public static class PeopleDAL
    {
        /// <summary>
        /// Adds a new person to the database.
        /// </summary>
        /// <param name="firstName">First name of the person.</param>
        /// <param name="lastName">Last name of the person.</param>
        /// <param name="status">Status of the person (default: Reporter).</param>
        public static void AddPerson(string firstName, string lastName, Enum.Status status = Enum.Status.Reporter)
        {
            DBConnection.Execute(
                $"INSERT INTO people (first_name, last_name, secret_code, type)\r\n" +
                $"VALUES ('{firstName}', '{lastName}', '{GenerateSecretCode(firstName, lastName)}', '{(int)status}')\r\n");
            Logger.Log($"New person created: {firstName} {lastName} (Status: {status})");
        }

        /// <summary>
        /// Generates a secret code for a person based on their name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <returns>A generated secret code.</returns>
        public static string GenerateSecretCode(string firstName, string lastName)
        {
            var code = $"{firstName.Substring(0, 2).ToUpper()}{lastName.Substring(0, 2).ToUpper()}{new Random().Next(100, 999)}";
            return code;
        }

        /// <summary>
        /// Retrieves people from the database by first and/or last name.
        /// </summary>
        /// <param name="firstName">First name (optional).</param>
        /// <param name="lastName">Last name (optional).</param>
        /// <returns>List of people matching the criteria.</returns>
        public static List<Dictionary<string, object>> GetPeople(string firstName = null, string lastName = null)
        {
            var sql = "SELECT * FROM people WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(firstName))
                sql += $" AND first_name = '{firstName}'";
            if (!string.IsNullOrWhiteSpace(lastName))
                sql += $" AND last_name = '{lastName}'";
            return DBConnection.Execute(sql);
        }

        /// <summary>
        /// Retrieves people from the database by secret code.
        /// </summary>
        /// <param name="secretCode">The secret code.</param>
        /// <returns>List of people matching the secret code.</returns>
        public static List<Dictionary<string, object>> GetPersonBySecretCode(string secretCode)
        {
            var sql = $"SELECT * FROM people WHERE secret_code = '{secretCode}'";
            return DBConnection.Execute(sql);
        }

        /// <summary>
        /// Retrieves people from the database by ID.
        /// </summary>
        /// <param name="Id">The person's ID.</param>
        /// <returns>List of people matching the ID.</returns>
        public static List<Dictionary<string, object>> GetPersonById(int Id)
        {
            var sql = $"SELECT * FROM people WHERE id = '{Id}'";
            return DBConnection.Execute(sql);
        }

        /// <summary>
        /// Gets the ID of a person by their secret code.
        /// </summary>
        /// <param name="secretCode">The secret code.</param>
        /// <returns>The person's ID, or -1 if not found.</returns>
        public static int GetPersonId(string secretCode)
        {
            var person = GetPersonBySecretCode(secretCode);
            if (person != null && person.Count > 0 && person[0].ContainsKey("id"))
            {
                return Convert.ToInt32(person[0]["id"]);
            }
            return -1;
        }

        /// <summary>
        /// Checks if a person exists in the database by secret code.
        /// </summary>
        /// <param name="secretCode">The secret code.</param>
        /// <returns>True if the person exists, otherwise false.</returns>
        public static bool IsPersonExists(string secretCode)
        {
            List<Dictionary<string, object>> person = GetPersonBySecretCode(secretCode);
            return person != null && person.Count > 0;
        }

        /// <summary>
        /// Returns all people with type = PotentialAgent.
        /// </summary>
        /// <returns>List of potential recruits.</returns>
        public static List<Dictionary<string, object>> GetPotentialRecruits()
        {
            return Malshinon.DB.DBConnection.Execute(
                $"SELECT id, first_name, last_name FROM people WHERE type = {(int)Enum.Status.PotentialAgent}");
        }

        /// <summary>
        /// Returns all targets with num_mentions >= 20.
        /// </summary>
        /// <returns>List of dangerous targets.</returns>
        public static List<Dictionary<string, object>> GetDangerousTargets()
        {
            return Malshinon.DB.DBConnection.Execute(
                $"SELECT id, first_name, last_name, num_mentions FROM people WHERE num_mentions >= 20");
        }
    }
}