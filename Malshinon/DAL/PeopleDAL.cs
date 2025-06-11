using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon.DAL
{
    internal class PeopleDAL
    {
        public PeopleDAL() { }

        public void AddPerson(string firstName, string lastName)
        {
            DBConnection.Execute(
                $"INSERT INTO people (first_name, last_name, secret_code, type)\r\n" +
                $"VALUES ('{firstName}', '{lastName}', '{genarateSecretCode(firstName, lastName)}', 'reporter')\r\n");
        }

        public string genarateSecretCode(string firstName, string lastName)
        {
            var code = $"{firstName.Substring(0, 2).ToUpper()}{lastName.Substring(0, 2).ToUpper()}";
            return code;
        }

        public List<Dictionary<string, object>> GetPeople(string firstName = null, string lastName = null)
        {
            var sql = "SELECT * FROM people WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(firstName))
                sql += $" AND first_name LIKE '%{firstName}%'";
            if (!string.IsNullOrWhiteSpace(lastName))
                sql += $" AND last_name LIKE '%{lastName}%'";
            
            return DBConnection.Execute(sql);
        }
    }
}
