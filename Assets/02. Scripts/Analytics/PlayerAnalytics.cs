using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어 관련 애널리틱스 매니저
/// 플레이어의 직업 선택, 행동 등을 추적
/// </summary>
public class PlayerAnalytics : MonoBehaviour
{
    /// <summary>
    /// 플레이어 직업 선택 이벤트 전송
    /// 플레이어가 직업을 선택할 때 호출
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="classType">선택한 직업 타입</param>
    public static void SendPlayerClassSelection(string playerId, string classType)
    {
        // 기본 파라미터 설정
        var parameters = new Dictionary<string, object>
        {
            ["Player_ID"] = playerId,
            ["Class_Type"] = classType,
            ["Selection_Time"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // 이벤트 전송
        AnalyticsManager.SendAnalyticsEvent("Player_Class_Info", parameters);
    }

    /// <summary>
    /// 플레이어 행동 이벤트 전송
    /// 플레이어가 특정 행동을 할 때 호출
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="actionType">행동 타입</param>
    /// <param name="stage">현재 스테이지</param>
    /// <param name="position">행동 위치 (선택)</param>
    /// <param name="extra">추가 정보 (선택)</param>
    public static void SendPlayerAction(string playerId, string actionType, int stage, string position = "", string extra = "")
    {
        // 기본 파라미터 설정
        var parameters = new Dictionary<string, object>
        {
            ["Player_ID"] = playerId,
            ["Action_Type"] = actionType,
            ["Stage"] = stage
        };

        // 선택적 파라미터 추가
        if (!string.IsNullOrEmpty(position)) parameters["Position"] = position;
        if (!string.IsNullOrEmpty(extra)) parameters["Extra"] = extra;

        // 이벤트 전송
        AnalyticsManager.SendAnalyticsEvent("Player_Action", parameters);
    }
} 