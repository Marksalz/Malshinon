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
            PersonIdentification();
            IntelSubmission();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static void PersonIdentification()
        {
            //PeopleDAL peopleDAL = new PeopleDAL();
            Console.WriteLine("Please enter your first name: ");
            string firstName = Console.ReadLine();
            Console.WriteLine("Please enter your last name: ");
            string lastName = Console.ReadLine();
            Dictionary<string, object> person = PeopleDAL.GetPeople(firstName, lastName).FirstOrDefault();
            if (person != null)
            {
                Console.WriteLine($"Welcome {person["first_name"]} {person["last_name"]}!");
                Console.WriteLine($"Your secret code is: {person["secret_code"]}");
            }
            else
            {
                Console.WriteLine("Person not found. adding person to DB");
                PeopleDAL.AddPerson(firstName, lastName);
            }
        }

        public static void IntelSubmission()
        {
            Console.WriteLine("Please enter your secret code: ");
            string secretCode = Console.ReadLine();
            if (!PeopleDAL.IsPersonExists(secretCode))
            {
                Console.WriteLine("Secret code not found. Please try again or register as a new person.");
                return;
            }
            Console.WriteLine("Please enter your report here: ");
            string report = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(report))
            {
                IntelDAL.SubmitReport(report, secretCode);
            }
            else
            {
                Console.WriteLine("Report cannot be empty. Please try again.");
            }
        }
    }
}
