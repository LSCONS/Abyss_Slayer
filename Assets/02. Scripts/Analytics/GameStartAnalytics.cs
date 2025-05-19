using UnityEngine;
using Unity.Services.Analytics;

public class GameStartAnalytics
{
    /// <summary>
    /// 게임 시작 시 플레이어 수, 난이도, 각 플레이어 직업 정보를 전송
    /// </summary>
    public static void SendStartUserInfo(
        int memberCount,
        // string difficulty,
        string player1Class, string player2Class,
        string player3Class, string player4Class, string player5Class)
    {
        var evt = new CustomEvent("Start_User_Info");
        evt["Member_Count"] = memberCount;
        // evt["Select_Difficulty"] = difficulty;
        evt["Player1_Class"] = player1Class;
        evt["Player2_Class"] = player2Class;
        evt["Player3_Class"] = player3Class;
        evt["Player4_Class"] = player4Class;
        evt["Player5_Class"] = player5Class;
        AnalyticsService.Instance.RecordEvent(evt);
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