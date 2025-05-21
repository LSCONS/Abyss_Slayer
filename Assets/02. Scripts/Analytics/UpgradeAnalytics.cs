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
            string stageNumber, string classType, string upgradeStat, int statLevel)
        {
            var evt = new CustomEvent("Class_Stat_Upgrade_Info");
            evt["Stage_Number"] = stageNumber;
            evt["Class_Type"] = classType;
            evt["Upgrade_Stat"] = upgradeStat;
            evt["Stat_Level"] = statLevel;
            AnalyticsService.Instance.RecordEvent(evt);
            Debug.LogAssertion($"[Analytics] Class_Stat_Upgrade_Info: Stage={stageNumber}, Class={classType}, Stat={upgradeStat}, Stat_Level={statLevel}");
        }

        /// <summary>
        /// 직업별 스킬 업그레이드 정보 전송
        /// </summary>
        public static void SendClassSkillUpgradeInfo(
            string stageNumber, string classType, string upgradeSkill, int skillLevel)
        {
            var evt = new CustomEvent("Class_Skill_Upgrade_Info");
            evt["Stage_Number"] = stageNumber;
            evt["Class_Type"] = classType;
            evt["Upgrade_Skill"] = upgradeSkill;
            evt["Skill_Level"] = skillLevel;
            AnalyticsService.Instance.RecordEvent(evt);
            Debug.LogAssertion($"[Analytics] Class_Skill_Upgrade_Info: Stage={stageNumber}, Class={classType}, Skill={upgradeSkill}, Skill_Level={skillLevel}");
        }
    }
} 