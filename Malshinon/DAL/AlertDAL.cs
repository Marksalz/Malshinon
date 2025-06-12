using Malshinon.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Malshinon.DAL
{
    /// <summary>
    /// Provides data access methods for managing alerts in the system.
    /// </summary>
    public static class AlertDAL
    {
        /// <summary>
        /// Adds a new alert for a target to the database.
        /// </summary>
        /// <param name="targetId">The ID of the target.</param>
        /// <param name="alertText">The alert message.</param>
        public static void AddAlert(int targetId, string alertText)
        {
            // Insert the alert into the database
            DBConnection.Execute(
                $"INSERT INTO alerts (target_id, alert_text) " +
                $"VALUES ({targetId}, '{alertText}')");
            Logger.Log($"Alert added for target ID {targetId}: {alertText}");
        }

        /// <summary>
        /// Checks if a target has reached the threshold for being considered dangerous.
        /// </summary>
        /// <param name="targetId">The ID of the target.</param>
        /// <returns>True if the target meets the threshold, otherwise false.</returns>
        public static bool CheckTargetThresholds(int targetId)
        {
            var targetStats = DBConnection.Execute(
                $"SELECT num_mentions FROM people WHERE id = {targetId}");

            if (targetStats.Count > 0)
            {
                int numMentions = Convert.ToInt32(targetStats[0]["num_mentions"] ?? 0);
                if (numMentions >= 20)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a target has been mentioned at least 3 times in the last 15 minutes.
        /// </summary>
        /// <param name="targetId">The ID of the target.</param>
        /// <returns>True if the recent mentions threshold is met, otherwise false.</returns>
        public static bool CheckRecentMentionsThreshold(int targetId)
        {
            var recentMentions = DBConnection.Execute(
                $"SELECT COUNT(*) as mention_count " +
                $"FROM intelreports " +
                $"WHERE target_id = {targetId} " +
                $"AND intel_timestamp >= DATE_SUB(NOW(), INTERVAL 15 MINUTE)");

            if (recentMentions.Count > 0)
            {
                int mentionCount = Convert.ToInt32(recentMentions[0]["mention_count"] ?? 0);
                if (mentionCount >= 3)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns all alerts in the system.
        /// </summary>
        /// <returns>List of all alerts.</returns>
        public static List<Dictionary<string, object>> GetAllAlerts()
        {
            return DBConnection.Execute("SELECT id, target_id, alert_text FROM alerts ORDER BY id DESC");
        }
    }
}
