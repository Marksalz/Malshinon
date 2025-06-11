using Malshinon.DB;
using Malshinon.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Malshinon.Services
{
    public static class AlertService
    {
        public static void genarateAlertIfNeeded(int targetId)
        {
            // Check target's thresholds
            bool isDangerous = AlertDAL.CheckTargetThresholds(targetId);

            // Check if target is mentioned 3+ times in a 15-minute window
            if (AlertDAL.CheckRecentMentionsThreshold(targetId))
            {
                isDangerous = true;
            }

            if (isDangerous)
            {
                string alertMsg = $"Potential threat alert: Target ID {targetId} is considered dangerous or high-priority.";
                AlertDAL.AddAlert(targetId, alertMsg);
                Logger.Log(alertMsg);
            }
        }


    }
}
