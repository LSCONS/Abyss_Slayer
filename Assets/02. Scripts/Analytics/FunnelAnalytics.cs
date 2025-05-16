using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임 퍼널(진행 단계) 추적을 위한 애널리틱스 매니저
/// 플레이어의 게임 진행 상황을 단계별로 추적
/// </summary>
public static class FunnelAnalytics
{
    /// <summary>
    /// 퍼널 단계 이벤트 전송 함수
    /// 플레이어가 특정 단계에 도달했을 때 호출
    /// </summary>
    /// <param name="stepNumber">퍼널 단계 번호 (1~23)</param>
    /// <param name="playerId">플레이어 ID (선택)</param>
    /// <param name="partyId">파티 ID (선택)</param>
    public static void SendFunnelStep(int stepNumber, string playerId = null, string partyId = null)
    {
        // 기본 파라미터 설정
        var parameters = new Dictionary<string, object>
        {
            ["Funnel_Step_Number"] = stepNumber
        };

        // 선택적 파라미터 추가
        if (!string.IsNullOrEmpty(playerId)) parameters["Player_ID"] = playerId;
        if (!string.IsNullOrEmpty(partyId)) parameters["Party_ID"] = partyId;

        // 이벤트 전송
        AnalyticsManager.SendAnalyticsEvent("Funnel_Step", parameters);
    }
} 