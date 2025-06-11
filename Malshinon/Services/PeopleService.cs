using System;
using System.Collections.Generic;
using System.Linq;
using Malshinon.DAL;

namespace Malshinon.Services
{
    public static class PeopleService
    {
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

        public static bool IsPersonExists(string secretCode)
        {
            return PeopleDAL.IsPersonExists(secretCode);
        }

        public static int GetPersonIdBySecretCode(string secretCode)
        {
            return PeopleDAL.GetPersonId(secretCode);
        }
    }
}
