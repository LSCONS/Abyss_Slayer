using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

/// <summary>
/// Unity Analytics의 기본 매니저 클래스
/// Unity Services 초기화 및 공통 이벤트 전송 기능을 제공
/// </summary>
public class AnalyticsManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static AnalyticsManager instance;
    private static bool isInitialized = false;

    // 외부에서 접근 가능한 인스턴스 프로퍼티
    public static AnalyticsManager Instance => instance;

    /// <summary>
    /// 싱글톤 패턴 구현을 위한 Awake 메서드
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Unity Services 초기화 및 데이터 수집 시작
    /// </summary>
    async void Start()
    {
        try
        {            
            await UnityServices.InitializeAsync();
            
            AnalyticsService.Instance.StartDataCollection();
            isInitialized = true;
            Debug.Log("[Analytics] Analytics data collection started successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogAssertion($"[Analytics] Unity Services initialization failed: {e.Message}\n{e.StackTrace}");
            isInitialized = false;
        }
    }

    /// <summary>
    /// 애널리틱스 서비스 초기화 상태 확인
    /// </summary>
    public static bool IsInitialized => isInitialized;

    /// <summary>
    /// 애널리틱스 이벤트를 전송하는 공통 메서드
    /// </summary>
    public static void SendAnalyticsEvent(string eventName, System.Collections.Generic.Dictionary<string, object> parameters)
    {
        if (!isInitialized)
        {
            Debug.LogAssertion($"[Analytics] Service not initialized. Event '{eventName}' not sent.");
            return;
        }

        try
        {
            var analyticsEvent = new CustomEvent(eventName);
            
            foreach (var param in parameters)
            {
                analyticsEvent[param.Key] = param.Value;
            }
            
            AnalyticsService.Instance.RecordEvent(analyticsEvent);
            Debug.Log($"[Analytics] Event '{eventName}' sent successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogAssertion($"[Analytics] Failed to send event '{eventName}': {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 퍼널 단계 이벤트 전송 함수
    /// </summary>
    /// <param name="stepNumber">퍼널 단계 번호 (1~23)</param>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="partyId">파티 ID</param>
    public static void SendFunnelStep(int stepNumber, string playerId = null, string partyId = null)
    {
        if (!isInitialized) return;

        var funnelEvent = new CustomEvent("Funnel_Step");
        funnelEvent["Funnel_Step_Number"] = stepNumber;
        if (!string.IsNullOrEmpty(playerId)) funnelEvent["Player_ID"] = playerId;
        if (!string.IsNullOrEmpty(partyId)) funnelEvent["Party_ID"] = partyId;
        AnalyticsService.Instance.RecordEvent(funnelEvent);
        Debug.Log($"[Analytics] Funnel Step Sent: {stepNumber}");
    }

    /// <summary>
    /// 플레이어 직업 선택 이벤트 전송
    /// </summary>
    public static void SendPlayerClassSelection(int playerIndex, string classType)
    {
        if (!isInitialized) return;

        var classEvent = new CustomEvent($"Player{playerIndex}_Class");
        classEvent["Class_Type"] = classType;
        
        AnalyticsService.Instance.RecordEvent(classEvent);
        Debug.Log($"[Analytics] Player {playerIndex} Class Info: {classType}");
    }

    /// <summary>
    /// 파티 구성 이벤트 전송
    /// </summary>
    /// <param name="partyId">파티 ID</param>
    /// <param name="partyMembers">파티원 정보 리스트 (Player_ID, Class_Type, Class_Level)</param>
    public static void SendPartyFormation(string partyId, List<PartyMemberInfo> partyMembers)
    {
        if (!isInitialized) return;

        var partyFormationEvent = new CustomEvent("Party_Formation");
        partyFormationEvent["Party_ID"] = partyId;
        partyFormationEvent["Member_Count"] = partyMembers.Count;
        
        // 파티원들의 직업 분포 계산
        var classDistribution = new Dictionary<string, int>();
        foreach (var member in partyMembers)
        {
            if (!classDistribution.ContainsKey(member.ClassType))
                classDistribution[member.ClassType] = 0;
            classDistribution[member.ClassType]++;
        }

        // 파티 구성 정보 추가
        string distributionString = string.Join(",", classDistribution.Select(x => $"{x.Key}:{x.Value}"));
        partyFormationEvent["Class_Distribution"] = distributionString;
        partyFormationEvent["Formation_Time"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // 파티원 상세 정보 (JSON 형식으로 저장)
        partyFormationEvent["Members_Detail"] = JsonUtility.ToJson(new PartyMembersWrapper { members = partyMembers });

        AnalyticsService.Instance.RecordEvent(partyFormationEvent);
        Debug.Log($"[Analytics] Party Formation: {partyId}, Members: {partyMembers.Count}, Distribution: {distributionString}");
    }
    
    /// <summary>
    /// 스탯 업그레이드 이벤트 전송
    /// </summary>
    public static void SendStatUpgradeEvent(int hpLevel, int hpIncrease, int damageLevel, int damageIncrease, int remainingPoints)
    {
        if (!isInitialized) return;

        var statEvent = new CustomEvent("Stat_Upgrade");
        statEvent["HP_Level"] = hpLevel;
        statEvent["HP_Increase"] = hpIncrease;
        statEvent["Damage_Level"] = damageLevel;
        statEvent["Damage_Increase"] = damageIncrease;
        statEvent["Remaining_Points"] = remainingPoints;
        
        AnalyticsService.Instance.RecordEvent(statEvent);
        Debug.Log($"[Analytics] Stat Upgrade Event sent: HP Lv.{hpLevel}(+{hpIncrease}), DMG Lv.{damageLevel}(+{damageIncrease})");
    }

    /// <summary>
    /// 스테이지 실패 정보 전송
    /// </summary>
    // public static void SendStageFailInfo(int stageNumber, int failTime)
    // {
    //     if (!isInitialized) return;

    //     var failEvent = new CustomEvent("Stage_Fail_Info");
    //     failEvent["Stage_Number"] = stageNumber;
    //     failEvent["Stage_Fail_Time"] = failTime;
        
    //     AnalyticsService.Instance.RecordEvent(failEvent);
    //     Debug.Log($"[Analytics] Stage Fail Info sent: Stage {stageNumber}, Time {failTime}s");
    // }

    /// <summary>
    /// 스킬 사용 정보 전송
    /// </summary>
    // public static void SendSkillUseInfo(string stageNumber, string classType, string skillName, int damage)
    // {
    //     if (!isInitialized) return;

    //     var skillEvent = new CustomEvent("Skill_Use_Info");
    //     skillEvent["Stage_Number"] = stageNumber;
    //     skillEvent["Class_Type"] = classType;
    //     skillEvent["Used_Skill_name"] = skillName;
    //     skillEvent["Skill_Attack_Damage"] = damage;
        
    //     AnalyticsService.Instance.RecordEvent(skillEvent);
    //     Debug.Log($"[Analytics] Skill Use Info sent: {skillName} by {classType} in {stageNumber}");
    // }

    /// <summary>
    /// 파티 진행 정보 전송
    /// </summary>
    /// <param name="partyId"></param>
    /// <param name="stage"></param>
    /// <param name="isClear"></param>
    /// <param name="failReason"></param>
    /// <param name="memberCount"></param>
    // public static void SendPartyProgress(string partyId, int stage, bool isClear, string failReason = "", int memberCount = 0)
    // {
    //     if (!isInitialized) return;

    //     var partyEvent = new CustomEvent("Party_Progress");
    //     partyEvent["Party_ID"] = partyId;
    //     partyEvent["Stage"] = stage;
    //     partyEvent["Is_Clear"] = isClear;
    //     if (!isClear && !string.IsNullOrEmpty(failReason)) partyEvent["Fail_Reason"] = failReason;
    //     partyEvent["Member_Count"] = memberCount;
    //     AnalyticsService.Instance.RecordEvent(partyEvent);
    // }


    [System.Serializable]
    private class PartyMembersWrapper
    {
        public List<PartyMemberInfo> members;
    }
}