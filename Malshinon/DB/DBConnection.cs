using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Malshinon.DB
{
    /// <summary>
    /// Provides methods for connecting to and interacting with the database.
    /// </summary>
    public static class DBConnection
    {
        /// <summary>
        /// Opens a new MySQL database connection.
        /// </summary>
        /// <param name="cs">Optional connection string.</param>
        /// <returns>An open MySqlConnection.</returns>
        public static MySqlConnection Connect(string cs = null)
        {
            var connStr = string.IsNullOrWhiteSpace(cs)
                ? "server=localhost;uid=root;database=malshinon"
                : cs;

            var conn = new MySqlConnection(connStr);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Closes the specified MySQL connection.
        /// </summary>
        /// <param name="conn">The connection to close.</param>
        public static void Disconnect(MySqlConnection conn) => conn.Close();

        /// <summary>
        /// Creates a MySqlCommand for the given SQL statement.
        /// </summary>
        /// <param name="sql">The SQL command text.</param>
        /// <returns>A MySqlCommand object.</returns>
        public static MySqlCommand Command(string sql)
        {
            var cmd = new MySqlCommand { CommandText = sql };
            return cmd;
        }

        /// <summary>
        /// Sends a command to the database and returns a MySqlDataReader.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static MySqlDataReader Send(MySqlConnection conn, MySqlCommand cmd)
        {
            cmd.Connection = conn;
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Parses the MySqlDataReader into a list of dictionaries.
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns></returns>
        private static List<Dictionary<string, object>> Parse(MySqlDataReader rdr)
        {
            var rows = new List<Dictionary<string, object>>();

            using (rdr)
            {
                while (rdr.Read())
                {
                    var row = new Dictionary<string, object>(rdr.FieldCount);
                    for (int i = 0; i < rdr.FieldCount; i++)
                        row[rdr.GetName(i)] = rdr.IsDBNull(i) ? null : rdr.GetValue(i);

                    rows.Add(row);
                }
            }
            return rows;
        }

        /// <summary>
        /// Executes a SQL query and returns the result as a list of dictionaries.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="connectionString">Optional connection string.</param>
        /// <returns>List of result rows as dictionaries.</returns>
        public static List<Dictionary<string, object>> Execute(string sql, string connectionString = null)
        {
            var conn = Connect(connectionString);
            var cmd = Command(sql);
            var rdr = Send(conn, cmd);
            return Parse(rdr);
        }

        /// <summary>
        /// Prints the result of a query to the console.
        /// </summary>
        /// <param name="keyValuePairs">The result set to print.</param>
        public static void PrintResult(List<Dictionary<string, object>> keyValuePairs)
        {
            if (keyValuePairs.Count == 0)
            {
                Console.WriteLine("No results found.");
                return;
            }

            foreach (var row in keyValuePairs)
            {
                foreach (var kv in row)
                    Console.WriteLine($"{kv.Key}: {kv.Value}");
                Console.WriteLine("---");
            }
        }

        /// <summary>
        /// Gets the number of recent mentions for a target in the last 15 minutes.
        /// </summary>
        /// <param name="targetId">The target's ID.</param>
        /// <returns>List containing the mention count.</returns>
        public static List<Dictionary<string, object>> GetRecentMentions(int targetId)
        {
            var sql = $"SELECT COUNT(*) as mention_count " +
                      $"FROM intelreports " +
                      $"WHERE target_id = {targetId} " +
                      $"AND timestamp >= DATE_SUB(NOW(), INTERVAL 15 MINUTE)";

            var recentMentions = Execute(sql);
            return recentMentions;
        }
    }
}
