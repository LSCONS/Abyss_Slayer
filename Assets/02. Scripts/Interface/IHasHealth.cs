using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHasHealth
{
    /// <summary>
    /// 체력이 변경될 때 호출되는 이벤트
    /// </summary>
    event Action<int, int> OnHpChanged;
    
    /// <summary>
    /// 체력을 설정하는 메서드
    /// </summary>
    /// <param name="hp">현재 체력</param>
    /// <param name="maxHp">최대 체력</param>
    void SetHp(int hp, int maxHp);
}