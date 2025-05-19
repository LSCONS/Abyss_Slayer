using UnityEngine;

/// <summary>
/// 파티원 정보를 담는 데이터 클래스
/// JSON 직렬화를 지원하여 애널리틱스 데이터 전송에 사용
/// </summary>
[System.Serializable]
public class PartyMemberInfo
{
    // 파티원의 이름
    public string PlayerName;

    // 파티원의 직업 타입
    public string ClassType;

    public PartyMemberInfo(string playerName, string classType)
    {
        PlayerName = playerName;
        ClassType = classType;
    }
}

// 엑셀 시트 기준 파티원 정보 구조체 없음. 필요시 여기에 추가. 