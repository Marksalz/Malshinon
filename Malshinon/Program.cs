using Malshinon.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malshinon
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.PersonIdentification();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public void PersonIdentification()
        {
            PeopleDAL peopleDAL = new PeopleDAL();
            Console.WriteLine("Please enter your first name: ");
            string firstName = Console.ReadLine();
            Console.WriteLine("Please enter your last name: ");
            string lastName = Console.ReadLine();
            Dictionary<string, object> person = peopleDAL.GetPeople(firstName, lastName).FirstOrDefault();
            if (person != null)
            {
                Console.WriteLine($"Welcome {person["first_name"]} {person["last_name"]}!");
                Console.WriteLine($"Your secret code is: {person["secret_code"]}");
            }
            else
            {
                Console.WriteLine("Person not found. adding person to DB");
                peopleDAL.AddPerson(firstName, lastName);
            }
        }
    }
}
