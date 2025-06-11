using UnityEngine;
using Unity.Services.Analytics;
using System.Collections.Generic;

public static class GameStartAnalytics
{
    /// <summary>
    /// 게임 시작 시 플레이어 수, 난이도, 각 플레이어 직업 정보를 전송
    /// </summary>
    public static void SendStartUserInfo(int memberCount, string player1Class, string player2Class, string player3Class, string player4Class, string player5Class)
    {
        if (!AnalyticsManager.IsInitialized) return;

        var evt = new CustomEvent("Start_User_Info");
        evt["Member_Count"] = memberCount;

        // null이 아닌 직업 정보만 파라미터에 추가
        if (!string.IsNullOrEmpty(player1Class)) evt["Player1_Class"] = player1Class;
        if (!string.IsNullOrEmpty(player2Class)) evt["Player2_Class"] = player2Class;
        if (!string.IsNullOrEmpty(player3Class)) evt["Player3_Class"] = player3Class;
        if (!string.IsNullOrEmpty(player4Class)) evt["Player4_Class"] = player4Class;
        if (!string.IsNullOrEmpty(player5Class)) evt["Player5_Class"] = player5Class;

        AnalyticsService.Instance.RecordEvent(evt);

#if AnalyticsDebug
        Debug.LogAssertion($"[Analytics] Start_User_Info: Member_Count={memberCount}, Player1={player1Class}, Player2={player2Class}, Player3={player3Class}, Player4={player4Class}, Player5={player5Class}");
#endif
    }

    //// <summary>
    //// 난이도 선택 정보 전송 (주석처리)
    //// </summary>
    //public static void SendDifficultySelection(string difficulty)
    //{
    //    var difficultyEvent = new CustomEvent("Select_Difficulty");
    //    difficultyEvent["Difficulty"] = difficulty;
    //    AnalyticsService.Instance.RecordEvent(difficultyEvent);
    //}
} 
