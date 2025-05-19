using UnityEngine;
using Unity.Services.Analytics;

public class SkillUseAnalytics
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
        Debug.LogAssertion($"[Analytics] Skill_Use_Info: Stage={stageNumber}, Class={classType}, Skill={skillName}, Damage={damage}, BackAttack={backAttackCount}");
    }
} 