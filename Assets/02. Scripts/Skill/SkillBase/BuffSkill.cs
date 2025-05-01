using UniRx;
using UnityEngine;

public enum BuffType
{
    None = 0,
    RogueDoubleShot = 1, //아처 더블 샷 버프
}

public class BuffSkill : Skill
{
    [field: Header("버프 초기화할 지속 시간")]
    [field: SerializeField]public ReactiveProperty<float> MaxBuffDuration { get; set; } // 최대 지속시간
        = new ReactiveProperty<float>(5f);
    [field: Header("버프 현재 남은 지속 시간(실시간)")]
    [field: SerializeField] public ReactiveProperty<float> CurBuffDuration { get; set; } // 현재 지속시간
        = new ReactiveProperty<float>(0f);
    [field: Header("버프 현재 적용 여부(실시간)")]
    [field: SerializeField] public bool IsApply { get; set; } = false; // 현재 버프 적용 여부
    [field: Header("버프 타입")]
    [field: SerializeField] public BuffType Type { get; private set; } = BuffType.None; // 버프 타입
}
