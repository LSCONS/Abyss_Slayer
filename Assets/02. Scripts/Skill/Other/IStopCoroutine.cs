using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStopCoroutine
{
    public Dictionary<SkillSlotKey, Skill> SkillDictionary { get; set; }
    public Coroutine HoldSkillCoroutine { get; set; }
    public Player Player { get; set; }
    public void StopCoroutine();
}

