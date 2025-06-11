using Malshinon.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Malshinon.DAL
{
    public static class AlertDAL
    {
        

        public static void AddAlert(int targetId, string alertText)
        {
            // Insert the alert into the database
            DBConnection.Execute(
                $"INSERT INTO alerts (target_id, alert_text) " +
                $"VALUES ({targetId}, '{alertText}')");
            Logger.Log($"Alert added for target ID {targetId}: {alertText}");
        }

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

    }
}
