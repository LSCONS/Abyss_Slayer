using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


/// <summary>
/// 버튼에 알람 뜨게 하는 컨트롤러 (스탯/스킬 포인트 등)
/// </summary>
public class PointsAlarmController : MonoBehaviour
{
    [Header("팝업 버튼")]
    [SerializeField] private UIPopupButton targetButton;

    [Header("포인트 타입 설정")]
    [SerializeField] private PointType pointType;

    public enum PointType
    {
        Stat,
        Skill
    }

    private async void Start()
    {
        if (targetButton == null) targetButton = GetComponent<UIPopupButton>();

        Player player = await ServerManager.Instance.WaitForThisPlayerAsync();
        if (player == null) { Debug.Log("플레이어 없음"); return; }


        // pointType에 따라서 추적할 ReactiveProperty를 선택함
        ReactiveProperty<int> targetProperty = pointType switch
        {
            PointType.Stat => player.StatPoint,
            PointType.Skill => player.SkillPoint,
            _ => throw new ArgumentOutOfRangeException()        // 예외처리
        };

        // targetProperty의 값 바뀔 때마다 구독 실행
        targetProperty
            .Subscribe(point => targetButton.OnAlarmIcon(point > 0))        // 0보다 클 때면 아이콘 킴
            .AddTo(this);   // 게임 오브젝트 파괴되면 해제
    }

}
