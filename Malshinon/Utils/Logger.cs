    using System;
    using System.IO;

    namespace Utils
    {
        /// <summary>
        /// Provides simple logging functionality to write messages to both the console and a log file.
        /// </summary>
        public static class Logger
        {
            /// <summary>
            /// Logs a message with a timestamp to the console and appends it to "log.txt".
            /// </summary>
            /// <param name="message">The message to log.</param>
            public static void Log(string message)
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                Console.WriteLine(logEntry);
                File.AppendAllText("log.txt", logEntry + Environment.NewLine);
            }

            /// <summary>
            /// Reads and returns the contents of "log.txt". Returns an empty string if the file does not exist.
            /// </summary>
            /// <returns>The contents of the log file, or an empty string if the file does not exist.</returns>
            public static string Read()
            {
                if (!File.Exists("log.txt"))
                {
                    return string.Empty;
                }
                return File.ReadAllText("log.txt");
            }
        }
    }