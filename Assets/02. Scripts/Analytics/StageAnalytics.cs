// ... 전체 코드 주석처리 ...

using UnityEngine;
using Unity.Services.Analytics;

public class StageAnalytics
{
    /// <summary>
    /// 스테이지 실패 정보 전송 (엑셀 시트 기준)
    /// </summary>
    // public static void SendStageFailInfo(
    //     int stageFail, int stageNumber, int failTime,
    //     string player1Class, string player2Class, string player3Class, string player4Class, string player5Class)
    // {
    //     var evt = new CustomEvent("Stage_Fail_Info");
    //     evt["Stage_Fail"] = stageFail;
    //     evt["Stage_Number"] = stageNumber;
    //     evt["Stage_Fail_Time"] = failTime;
    //     evt["Player1_Class"] = player1Class;
    //     evt["Player2_Class"] = player2Class;
    //     evt["Player3_Class"] = player3Class;
    //     evt["Player4_Class"] = player4Class;
    //     evt["Player5_Class"] = player5Class;
    //     AnalyticsService.Instance.RecordEvent(evt);
    // }

    /// <summary>
    /// 스테이지 클리어 정보 전송 (엑셀 시트 기준)
    /// </summary>
    // public static void SendStageClearInfo(
    //     int stageClear, int stageNumber, float clearTime,
    //     string player1Class, int player1Damage, int player1Death,
    //     string player2Class, int player2Damage, int player2Death,
    //     string player3Class, int player3Damage, int player3Death,
    //     string player4Class, int player4Damage, int player4Death,
    //     string player5Class, int player5Damage, int player5Death)
    // {
    //     var evt = new CustomEvent("Stage_Clear_Info");
    //     evt["Stage_Clear"] = stageClear;
    //     evt["Stage_Number"] = stageNumber;
    //     evt["Stage_Clear_Time"] = clearTime;
    //     evt["Player1_Class"] = player1Class;
    //     evt["Player1_Damage"] = player1Damage;
    //     evt["Player1_Death"] = player1Death;
    //     evt["Player2_Class"] = player2Class;
    //     evt["Player2_Damage"] = player2Damage;
    //     evt["Player2_Death"] = player2Death;
    //     evt["Player3_Class"] = player3Class;
    //     evt["Player3_Damage"] = player3Damage;
    //     evt["Player3_Death"] = player3Death;
    //     evt["Player4_Class"] = player4Class;
    //     evt["Player4_Damage"] = player4Damage;
    //     evt["Player4_Death"] = player4Death;
    //     evt["Player5_Class"] = player5Class;
    //     evt["Player5_Damage"] = player5Damage;
    //     evt["Player5_Death"] = player5Death;
    //     AnalyticsService.Instance.RecordEvent(evt);
    // }

    /// <summary>
    /// 모든 스테이지 올클리어 정보 전송 (엑셀 시트 기준)
    /// </summary>
    // public static void SendStageAllClearInfo(
    //     int stageAllClear, int stageAllClearTime,
    //     string player1Class, int player1Damage, int player1Death,
    //     string player2Class, int player2Damage, int player2Death,
    //     string player3Class, int player3Damage, int player3Death,
    //     string player4Class, int player4Damage, int player4Death,
    //     string player5Class, int player5Damage, int player5Death)
    // {
    //     var evt = new CustomEvent("Stage_All_Clear_Info");
    //     evt["Stage_All_Clear"] = stageAllClear;
    //     evt["Stage_All_Clear_Time"] = stageAllClearTime;
    //     evt["Player1_Class"] = player1Class;
    //     evt["Stage_All_Player1_Damage"] = player1Damage;
    //     evt["Stage_All_Player1_Death"] = player1Death;
    //     evt["Player2_Class"] = player2Class;
    //     evt["Stage_All_Player2_Damage"] = player2Damage;
    //     evt["Stage_All_Player2_Death"] = player2Death;
    //     evt["Player3_Class"] = player3Class;
    //     evt["Stage_All_Player3_Damage"] = player3Damage;
    //     evt["Stage_All_Player3_Death"] = player3Death;
    //     evt["Player4_Class"] = player4Class;
    //     evt["Stage_All_Player4_Damage"] = player4Damage;
    //     evt["Stage_All_Player4_Death"] = player4Death;
    //     evt["Player5_Class"] = player5Class;
    //     evt["Stage_All_Player5_Damage"] = player5Damage;
    //     evt["Stage_All_Player5_Death"] = player5Death;
    //     AnalyticsService.Instance.RecordEvent(evt);
    // }

    /// <summary>
    /// 스테이지 보스 처치 정보 전송
    /// </summary>
    // public static void SendBossKillInfo(int stageNumber, string bossType, float killTime)
    // {
    //     var bossKillEvent = new CustomEvent("Boss_Kill_Info");
    //     bossKillEvent["Stage_Number"] = stageNumber;
    //     bossKillEvent["Boss_Type"] = bossType;
    //     bossKillEvent["Kill_Time"] = killTime;
    //     AnalyticsService.Instance.RecordEvent(bossKillEvent);
    // }
} 