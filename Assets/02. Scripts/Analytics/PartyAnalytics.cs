using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 게임오버 원인을 정의하는 enum
/// </summary>
public enum GameOverReason
{
    TimeOut,    // 제한 시간 초과
    AllPlayerDead,       // 모든 파티원 사망
    Unknown              // 기타 원인
}

/// <summary>
/// 파티 관련 애널리틱스 매니저
/// 파티 구성, 파티원 정보 등을 추적
/// </summary>
public class PartyAnalytics : MonoBehaviour
{
    private static Dictionary<string, float> stageStartTimes = new Dictionary<string, float>();

    /// <summary>
    /// 스테이지 시작 시간을 기록
    /// </summary>
    /// <param name="partyId">파티 ID</param>
    public static void RecordStageStart(string partyId)
    {
        stageStartTimes[partyId] = Time.time;
    }

    /// <summary>
    /// 파티 구성 이벤트 전송
    /// 파티가 구성되거나 변경될 때 호출
    /// </summary>
    /// <param name="partyId">파티 ID</param>
    /// <param name="partyMembers">파티원 정보 리스트 (닉네임, 직업)</param>
    /// <param name="stage">현재 스테이지</param>
    public static void SendPartyFormation(string partyId, List<(string nickname, string classType)> partyMembers, int stage)
    {
        // 기본 파라미터 설정
        var parameters = new Dictionary<string, object>
        {
            ["Party_ID"] = partyId,
            ["Member_Count"] = partyMembers.Count,
            ["Stage"] = stage,
            ["Formation_Time"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // 파티원 정보 추가
        for (int i = 0; i < partyMembers.Count; i++)
        {
            parameters[$"Member_{i + 1}_Name"] = partyMembers[i].nickname;
            parameters[$"Member_{i + 1}_Class"] = partyMembers[i].classType;
        }

        // 이벤트 전송
        AnalyticsManager.SendAnalyticsEvent("Party_Formation", parameters);
    }

    /// <summary>
    /// 파티 클리어 이벤트 전송
    /// 파티가 스테이지를 클리어했을 때 호출
    /// </summary>
    /// <param name="partyId">파티 ID</param>
    /// <param name="stage">클리어한 스테이지</param>
    /// <param name="survivingMembers">생존한 파티원 수</param>
    public static void SendPartyClear(string partyId, int stage, int survivingMembers)
    {
        // 클리어 시간 계산
        float clearTime = 0f;
        if (stageStartTimes.ContainsKey(partyId))
        {
            clearTime = Time.time - stageStartTimes[partyId];
            stageStartTimes.Remove(partyId); // 사용한 시작 시간 제거
        }

        var parameters = new Dictionary<string, object>
        {
            ["Party_ID"] = partyId,
            ["Stage"] = stage,
            ["Clear_Time"] = clearTime,
            ["Surviving_Members"] = survivingMembers,
            ["Clear_Time_Stamp"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        AnalyticsManager.SendAnalyticsEvent("Party_Clear", parameters);
    }

    /// <summary>
    /// 파티 게임오버 이벤트 전송
    /// 파티가 게임오버되었을 때 호출
    /// </summary>
    /// <param name="partyId">파티 ID</param>
    /// <param name="stage">실패한 스테이지</param>
    /// <param name="reason">게임오버 원인</param>
    /// <param name="remainingTime">남은 시간 (제한 시간 초과인 경우)</param>
    /// <param name="deadMembers">사망한 파티원 수 (전원 사망인 경우)</param>
    public static void SendPartyGameOver(string partyId, int stage, GameOverReason reason, float remainingTime = 0f, int deadMembers = 0)
    {
        // 게임오버 시간 계산
        float playTime = 0f;
        if (stageStartTimes.ContainsKey(partyId))
        {
            playTime = Time.time - stageStartTimes[partyId];
            stageStartTimes.Remove(partyId); // 사용한 시작 시간 제거
        }

        var parameters = new Dictionary<string, object>
        {
            ["Party_ID"] = partyId,
            ["Stage"] = stage,
            ["Play_Time"] = playTime,
            ["GameOver_Reason"] = reason.ToString(),
            ["GameOver_Time_Stamp"] = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // 게임오버 원인에 따른 추가 정보
        switch (reason)
        {
            case GameOverReason.TimeOut:
                parameters["Remaining_Time"] = remainingTime;
                break;
            case GameOverReason.AllPlayerDead:
                parameters["Dead_Members_Count"] = deadMembers;
                break;
        }

        AnalyticsManager.SendAnalyticsEvent("Party_GameOver", parameters);
    }

    /// <summary>
    /// 파티원들의 직업 분포를 계산하는 헬퍼 메서드
    /// </summary>
    /// <param name="members">파티원 리스트</param>
    /// <returns>직업별 인원 수 딕셔너리</returns>
    private static Dictionary<string, int> CalculateClassDistribution(List<PartyMemberInfo> members)
    {
        // 직업별 인원 수를 저장할 딕셔너리
        var distribution = new Dictionary<string, int>();
        
        // 각 파티원의 직업별로 카운트
        foreach (var member in members)
        {
            if (!distribution.ContainsKey(member.ClassType))
                distribution[member.ClassType] = 0;
            distribution[member.ClassType]++;
        }
        
        return distribution;
    }
} 