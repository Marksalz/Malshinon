using Malshinon.DAL;
using Malshinon.Services;
using Malshinon.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Malshinon
{
    internal class Program
    {
        private static Dictionary<string, object> _currentUser;

        static void Main(string[] args)
        {
            ShowWelcomeScreen();
            MainMenu();
        }

        private static void ShowWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine("===============================================");
            Console.WriteLine("         Welcome to the Malshinon System       ");
            Console.WriteLine("===============================================");
            Console.WriteLine();
            Console.WriteLine("You may log in at any time to access user-specific features.");
            Console.WriteLine();
        }

        private static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===============================================");
                Console.WriteLine("                   Main Menu                   ");
                Console.WriteLine("===============================================");
                Console.WriteLine("1. Import reports from CSV file");
                Console.WriteLine("2. Log in / Switch user");
                Console.WriteLine("3. Submit a new report");
                Console.WriteLine("4. View your secret code");
                Console.WriteLine("5. Clear all tables in the database");
                Console.WriteLine("6. Exit");
                Console.WriteLine("===============================================");
                if (_currentUser != null)
                {
                    Console.WriteLine($"Current user: {_currentUser["first_name"]} {_currentUser["last_name"]}");
                }
                else
                {
                    Console.WriteLine("No user logged in.");
                }
                Console.Write("Enter your choice (1-6): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        InitializeDBFromCSV.ImportReportsFromCsv();
                        Pause();
                        break;
                    case "2":
                        PersonIdentification();
                        Pause();
                        break;
                    case "3":
                        IntelSubmission();
                        Pause();
                        break;
                    case "4":
                        ShowSecretCode();
                        Pause();
                        break;
                    case "5":
                        ClearAllTables();
                        Pause();
                        break;
                    case "6":
                        Console.WriteLine("Thank you for using Malshinon. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Pause();
                        break;
                }
            }
        }

        public static void PersonIdentification()
        {
            Console.Write("Please enter your first name: ");
            string firstName = Console.ReadLine();
            Console.Write("Please enter your last name: ");
            string lastName = Console.ReadLine();
            var person = PeopleDAL.GetPeople(firstName, lastName).FirstOrDefault();
            if (person != null)
            {
                _currentUser = person;
                Console.WriteLine($"Welcome {person["first_name"]} {person["last_name"]}!");
                Console.WriteLine($"Your secret code is: {person["secret_code"]}");
            }
            else
            {
                Console.WriteLine("Person not found. Adding person to DB...");
                PeopleDAL.AddPerson(firstName, lastName);
                _currentUser = PeopleDAL.GetPeople(firstName, lastName).FirstOrDefault();
                Console.WriteLine($"Welcome {_currentUser["first_name"]} {_currentUser["last_name"]}!");
                Console.WriteLine($"Your secret code is: {_currentUser["secret_code"]}");
            }
        }

        public static void IntelSubmission()
        {
            if (_currentUser == null)
            {
                Console.WriteLine("No user identified. Please log in first (option 1).");
                return;
            }
            string secretCode = _currentUser["secret_code"].ToString();
            Console.WriteLine();
            Console.WriteLine("Please enter your report below:");
            string report = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(report))
            {
                IntelService.SubmitReport(report, secretCode);
                Console.WriteLine("Report submitted successfully!");
            }
            else
            {
                Console.WriteLine("Report cannot be empty. Please try again.");
            }
        }

       

        private static void ShowSecretCode()
        {
            if (_currentUser != null)
            {
                Console.WriteLine($"Your secret code is: {_currentUser["secret_code"]}");
            }
            else
            {
                Console.WriteLine("No user identified. Please log in first (option 1).");
            }
        }

        private static void ClearAllTables()
        {
            Console.WriteLine("WARNING: This will delete ALL data from the database (people, intelreports, alerts).");
            Console.Write("Are you sure you want to continue? (yes/no): ");
            string confirm = Console.ReadLine();
            if (confirm?.Trim().ToLower() == "yes")
            {
                try
                {
                    // Order matters due to foreign key constraints
                    Malshinon.DB.DBConnection.Execute("DELETE FROM alerts");
                    Malshinon.DB.DBConnection.Execute("DELETE FROM intelreports");
                    Malshinon.DB.DBConnection.Execute("DELETE FROM people");
                    Console.WriteLine("All tables have been cleared.");
                    _currentUser = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error clearing tables: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Operation cancelled.");
            }
        }

       

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}