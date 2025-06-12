using System;
using System.Collections.Generic;
using System.Linq;
using Malshinon.DAL;

namespace Malshinon.Services
{
    /// <summary>
    /// Provides high-level services for managing people in the system.
    /// </summary>
    public static class PeopleService
    {
        /// <summary>
        /// Finds a person by name or creates them if they do not exist.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <returns>The person's data as a dictionary.</returns>
        public static Dictionary<string, object> FindOrCreatePerson(string firstName, string lastName)
        {
            Dictionary<string, object> person = PeopleDAL.GetPeople(firstName, lastName).FirstOrDefault();
            if (person == null)
            {
                PeopleDAL.AddPerson(firstName, lastName, Enum.Status.Reporter);
                person = PeopleDAL.GetPeople(firstName, lastName).FirstOrDefault();
            }
            return person;
        }

        /// <summary>
        /// Checks if a person exists by secret code.
        /// </summary>
        /// <param name="secretCode">The secret code.</param>
        /// <returns>True if the person exists, otherwise false.</returns>
        public static bool IsPersonExists(string secretCode)
        {
            return PeopleDAL.IsPersonExists(secretCode);
        }

        /// <summary>
        /// Gets a person's ID by their secret code.
        /// </summary>
        /// <param name="secretCode">The secret code.</param>
        /// <returns>The person's ID.</returns>
        public static int GetPersonIdBySecretCode(string secretCode)
        {
            return PeopleDAL.GetPersonId(secretCode);
        }
    }
}
