using UnityEngine;
using Unity.Services.Analytics;

public class SkillAnalytics
{
    /// <summary>
    /// 스킬 사용 정보 전송 (엑셀 시트 기준)
    /// </summary>
    public static void SendSkillUseInfo(
        string stageNumber, string classType, string skillName, int damage, int backAttackCount)
    {
        var evt = new CustomEvent("Skill_Use_Info");
        evt["Stage_Number"] = stageNumber;
        evt["Class_Type"] = classType;
        evt["Used_Skill_name"] = skillName;
        evt["Skill_Attack_Damage"] = damage;
        evt["Back_Attack_Count"] = backAttackCount;
        AnalyticsService.Instance.RecordEvent(evt);
    }

    /// <summary>
    /// 직업별 스탯 분배에 대한 정보 전송
    /// </summary>
    public static void SendClassStateUpgradeInfo(
        string stageNumber, string classType, string upgradeState, string upgradeState2)
    {
        var evt = new CustomEvent("Class_State_Upgrade_Info");
        evt["Stage_Number"] = stageNumber;
        evt["Class_Type"] = classType;
        evt["Upgrade_State"] = upgradeState;
        evt["Upgrade_State2"] = upgradeState2;
        AnalyticsService.Instance.RecordEvent(evt);
    }

    /// <summary>
    /// 직업별 스킬 강화 분배에 대한 정보 전송
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