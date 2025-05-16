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

    // 외부에서 접근 가능한 인스턴스 프로퍼티
    public static AnalyticsManager Instance => instance;

    /// <summary>
    /// 싱글톤 패턴 구현을 위한 Awake 메서드
    /// </summary>
    private void Awake()
    {
        // 인스턴스가 없으면 현재 오브젝트를 인스턴스로 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // 이미 인스턴스가 있다면 현재 오브젝트 제거
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
            // Unity Services 초기화
            await UnityServices.InitializeAsync();
            // 데이터 수집 시작
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("Unity Services Initialized");

            // 예시: 게임 시작 시 퍼널 1단계 전송
            SendFunnelStep(1);
        }
        catch (System.Exception e)
        {
            // 초기화 실패 시 에러 로그 출력
            Debug.LogError("Unity Services failed to initialize: " + e.Message);
        }
    }

    /// <summary>
    /// 퍼널 단계 이벤트 전송 함수
    /// </summary>
    /// <param name="stepNumber">퍼널 단계 번호 (1~23)</param>
    /// <param name="playerId">플레이어 ID (선택)</param>
    /// <param name="partyId">파티 ID (선택)</param>
    public static void SendFunnelStep(int stepNumber, string playerId = null, string partyId = null)
    {
        var funnelEvent = new CustomEvent("Funnel_Step");
        funnelEvent["Funnel_Step_Number"] = stepNumber;
        if (!string.IsNullOrEmpty(playerId)) funnelEvent["Player_ID"] = playerId;
        if (!string.IsNullOrEmpty(partyId)) funnelEvent["Party_ID"] = partyId;
        AnalyticsService.Instance.RecordEvent(funnelEvent);
        Debug.Log($"[Analytics] Funnel Step Sent: {stepNumber}");
    }

    public static void SendPartyProgress(string partyId, int stage, bool isClear, string failReason = "", int memberCount = 0)
    {
        var partyEvent = new CustomEvent("Party_Progress");
        partyEvent["Party_ID"] = partyId;
        partyEvent["Stage"] = stage;
        partyEvent["Is_Clear"] = isClear;
        if (!isClear && !string.IsNullOrEmpty(failReason)) partyEvent["Fail_Reason"] = failReason;
        partyEvent["Member_Count"] = memberCount;
        AnalyticsService.Instance.RecordEvent(partyEvent);
    }

    public static void SendPlayerAction(string playerId, string actionType, int stage, string position = "", string extra = "")
    {
        var actionEvent = new CustomEvent("Player_Action");
        actionEvent["Player_ID"] = playerId;
        actionEvent["Action_Type"] = actionType;
        actionEvent["Stage"] = stage;
        if (!string.IsNullOrEmpty(position)) actionEvent["Position"] = position;
        if (!string.IsNullOrEmpty(extra)) actionEvent["Extra"] = extra;
        AnalyticsService.Instance.RecordEvent(actionEvent);
    }

    /// <summary>
    /// 플레이어 직업 선택 이벤트 전송
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="classType">직업 타입 (예: "Warrior", "Mage", "Archer" 등)</param>
    /// <param name="classLevel">직업 레벨</param>
    public static void SendPlayerClassSelection(string playerId, string classType, int classLevel)
    {
        var classEvent = new CustomEvent("Player_Class_Info");
        classEvent["Player_ID"] = playerId;
        classEvent["Class_Type"] = classType;
        classEvent["Selection_Time"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        AnalyticsService.Instance.RecordEvent(classEvent);
        Debug.Log($"[Analytics] Player Class Info: {playerId}, {classType}, Level {classLevel}");
    }

    /// <summary>
    /// 파티 구성 이벤트 전송
    /// </summary>
    /// <param name="partyId">파티 ID</param>
    /// <param name="partyMembers">파티원 정보 리스트 (Player_ID, Class_Type, Class_Level)</param>
    public static void SendPartyFormation(string partyId, List<PartyMemberInfo> partyMembers)
    {
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
        partyFormationEvent["Class_Distribution"] = string.Join(",", classDistribution.Select(x => $"{x.Key}:{x.Value}"));
        partyFormationEvent["Formation_Time"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        
        // 파티원 상세 정보 (JSON 형식으로 저장)
        partyFormationEvent["Members_Detail"] = JsonUtility.ToJson(new PartyMembersWrapper { members = partyMembers });

        AnalyticsService.Instance.RecordEvent(partyFormationEvent);
        Debug.Log($"[Analytics] Party Formation: {partyId}, Members: {partyMembers.Count}");
    }

    [System.Serializable]
    private class PartyMembersWrapper
    {
        public List<PartyMemberInfo> members;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 애널리틱스 이벤트를 전송하는 공통 메서드
    /// </summary>
    /// <param name="eventName">이벤트 이름</param>
    /// <param name="parameters">이벤트 파라미터 딕셔너리</param>
    public static void SendAnalyticsEvent(string eventName, Dictionary<string, object> parameters)
    {
        // 새로운 커스텀 이벤트 생성
        var analyticsEvent = new CustomEvent(eventName);
        
        // 파라미터들을 이벤트에 추가
        foreach (var param in parameters)
        {
            analyticsEvent[param.Key] = param.Value;
        }
        
        // 이벤트 전송
        AnalyticsService.Instance.RecordEvent(analyticsEvent);
        
        // 디버그 로그 출력
        Debug.Log($"[Analytics] {eventName} Sent: {string.Join(", ", parameters.Select(x => $"{x.Key}:{x.Value}"))}");
    }
}