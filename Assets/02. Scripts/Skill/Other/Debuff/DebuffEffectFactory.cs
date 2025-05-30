using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 디버프 타입에 맞는 디버프 효과 적용하는 클래스
/// </summary>
public static class DebuffEffectFactory
{
    public static IDebuff Create(EBuffType type)
    {
        switch (type)
        {
            case EBuffType.Weaken:
                return new WeakenDebuff();
            default:
                Debug.LogWarning($"이상한 디버프 들어옴");
                return null;
        }
    }
}
