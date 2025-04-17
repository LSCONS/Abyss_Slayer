using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Flags]
public enum BuffType
{
    None = 0,
    ArchorDoubleShot = 1 << 0,  //아처 더블 샷 버프
}

public class BuffSkill : Skill
{
    public ReactiveProperty<float> maxBuffDuration { get; set; } = new ReactiveProperty<float>(5f);              //최대 지속시간
    public ReactiveProperty<float> curBuffDuration { get; set; } = new ReactiveProperty<float>(0f);              //현재 지속시간
    public bool isApply = false;
    public BuffType type = BuffType.None;       //현재 버프 타입
}
