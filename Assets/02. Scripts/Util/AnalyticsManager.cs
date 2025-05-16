using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        try
        {
            // Unity Services 초기화
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();

            Debug.Log("Unity Services Initialized");

            // 예시: 게임 시작 시 퍼널 1단계 전송
            SendFunnelStep(1);
        }
        catch (System.Exception e)
        {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
