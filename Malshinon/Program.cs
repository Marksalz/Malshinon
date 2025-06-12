using Malshinon.DAL;
using Malshinon.DB;
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
            //resetLogFile();
            ShowWelcomeScreen();
            Pause();
            MainMenu();
        }

        /// <summary>
        /// The welcome screen that is displayed when the program starts.
        /// </summary>
        public static void ShowWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine("===============================================");
            Console.WriteLine("         Welcome to the Malshinon System       ");
            Console.WriteLine("===============================================");
            Console.WriteLine();
            Console.WriteLine("You may log in at any time to access user-specific features.");
            Console.WriteLine();
        }

        /// <summary>
        /// The main menu of the application, where users can choose different options.
        /// </summary>
        public static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===============================================");
                Console.WriteLine("                   Main Menu                   ");
                Console.WriteLine("===============================================");
                Console.WriteLine("1. Import reports from CSV file");
                Console.WriteLine("2. Submit a new report");
                Console.WriteLine("3. View your secret code");
                Console.WriteLine("4. Clear all tables in the database");
                Console.WriteLine("5. Show dashboard");
                Console.WriteLine("6. Exit");
                Console.WriteLine("===============================================");

                Console.Write("Enter your choice (1-7): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        InitializeDBFromCSV.ImportReportsFromCsv();
                        Pause();
                        break;
                    case "2":
                        IntelSubmission();
                        Pause();
                        break;
                    case "3":
                        ShowSecretCode();
                        Pause();
                        break;
                    case "4":
                        ClearAllTables();
                        Pause();
                        break;
                    case "5":
                        ShowDashboard();
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


        /// <summary>
        /// This method identifies the user by their first and last name. 
        /// If the user is not found in the database, they are added as a new person.
        /// </summary>
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
            }
            else
            {
                Console.WriteLine("Person not found. Adding person to DB...");
                PeopleDAL.AddPerson(firstName, lastName);
                _currentUser = PeopleDAL.GetPeople(firstName, lastName).FirstOrDefault();
                Console.WriteLine($"Welcome {_currentUser["first_name"]} {_currentUser["last_name"]}!");
            }
        }

        /// <summary>
        /// The method to submit an intelligence report.
        /// </summary>
        public static void IntelSubmission()
        {
            PersonIdentification();
            string secretCode = _currentUser["secret_code"].ToString();
            Console.WriteLine();
            Console.WriteLine("Please enter your report below:");
            string report = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(report))
            {
                IntelService.SubmitReport(report, secretCode);
                Console.WriteLine("Report submitted successfully!");
                _currentUser = null;
            }
            else
            {
                Console.WriteLine("Report cannot be empty. Please try again.");
            }
        }

        /// <summary>
        /// The method to show the user's secret code.
        /// </summary>
        private static void ShowSecretCode()
        {
            // Ensure user is identified before showing secret code
            PersonIdentification();
            Console.WriteLine($"Your secret code is: {_currentUser["secret_code"]}");
            _currentUser = null;
        }

        /// <summary>
        /// A method to clear all tables in the database. Only bening used for testing purposes.
        /// </summary>
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
                    DBConnection.Execute("DELETE FROM alerts");
                    DBConnection.Execute("ALTER TABLE alerts AUTO_INCREMENT = 1");

                    DBConnection.Execute("DELETE FROM intelreports");
                    DBConnection.Execute("ALTER TABLE intelreports AUTO_INCREMENT = 1");

                    DBConnection.Execute("DELETE FROM people");
                    DBConnection.Execute("ALTER TABLE people AUTO_INCREMENT = 1");

                    Console.WriteLine("All tables have been cleared and auto-increment values reset.");
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

        /// <summary>
        /// This method displays the dashboard with potential recruits, dangerous targets, and all alerts.
        /// </summary>
        public static void ShowDashboard()
        {
            Console.Clear();
            Console.WriteLine("============== DASHBOARD ==============");
            Console.WriteLine();

            // Potential Recruits
            Console.WriteLine("Potential Recruits (Potential Agents):");
            var recruits = PeopleDAL.GetPotentialRecruits();
            if (recruits.Count == 0)
                Console.WriteLine("  None found.");
            else
                DBConnection.PrintResult(recruits);

            Console.WriteLine();

            // Dangerous Targets
            Console.WriteLine("Dangerous Targets:");
            var dangerousTargets = PeopleDAL.GetDangerousTargets();
            if (dangerousTargets.Count == 0)
                Console.WriteLine("  None found.");
            else
                DBConnection.PrintResult(dangerousTargets);

            Console.WriteLine();

            // All Alerts
            Console.WriteLine("All Alerts:");
            var alerts = AlertDAL.GetAllAlerts();
            if (alerts.Count == 0)
                Console.WriteLine("  No alerts found.");
            else
                DBConnection.PrintResult(alerts);

            Console.WriteLine("=======================================");
        }

        /// <summary>
        /// this method pauses the console and waits for the user to press any key to continue.
        /// </summary>
        public static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// A method to reset the log file by clearing its contents.
        /// </summary>
        public static void resetLogFile()
        {
            string logFilePath = "log.txt";
            if (File.Exists(logFilePath))
            {
                File.WriteAllText(logFilePath, string.Empty);
                Console.WriteLine("Log file has been reset.");
            }
            else
            {
                Console.WriteLine("Log file does not exist.");
            }
        }
    }
}