using UnityEngine;
using Unity.Services.Analytics;

namespace Analytics
{
    public class UpgradeAnalytics
    {
        /// <summary>
        /// 직업별 스탯 업그레이드 정보 전송
        /// </summary>
        public static void SendClassStatUpgradeInfo(
            string stageNumber, string classType, string upgradeStat)
        {
            var evt = new CustomEvent("Class_State_Upgrade_Info");
            evt["Stage_Number"] = stageNumber;
            evt["Class_Type"] = classType;
            evt["Upgrade_Stat"] = upgradeStat;
            AnalyticsService.Instance.RecordEvent(evt);
        }

        /// <summary>
        /// 직업별 스킬 업그레이드 정보 전송
        /// </summary>
        public static void SendClassSkillUpgradeInfo(
            string stageNumber, string classType, string upgradeSkill)
        {
            var evt = new CustomEvent("Class_Skill_Upgrade_Info");
            evt["Stage_Number"] = stageNumber;
            evt["Class_Type"] = classType;
            evt["Upgrade_Skill"] = upgradeSkill;
            AnalyticsService.Instance.RecordEvent(evt);
        }
    }
} 