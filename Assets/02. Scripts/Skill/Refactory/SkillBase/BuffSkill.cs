using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public enum BuffType
{
    None = 0,
    ArcherDoubleShot = 1,  //아처 더블 샷 버프
}

public class BuffSkill : Skill
{
    [field: SerializeField]public ReactiveProperty<float> MaxBuffDuration { get; set; }     //최대 지속시간
        = new ReactiveProperty<float>(5f);    
    [field: SerializeField] public ReactiveProperty<float> CurBuffDuration { get; set; }    //현재 지속시간
        = new ReactiveProperty<float>(0f);     
    [field: SerializeField] public bool IsApply { get; set; } = false;                      //현재 버프 적용 여부
    [field: SerializeField] public BuffType Type { get; private set; } = BuffType.None;     //버프 타입
}
