using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum BuffType
{
    None = 0,
    ArchorDoubleShot = 1 << 0,  //아처 더블 샷 버프
}

public class BuffSkill : Skill
{
    public float maxBuffDuration = 5f;              //최대 지속시간
    public float curBuffDuration = 0f;              //현재 지속시간
    public BuffType type = BuffType.None;       //현재 버프 타입
}
