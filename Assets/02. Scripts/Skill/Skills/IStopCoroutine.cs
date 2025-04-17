using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStopCoroutine
{
    public Dictionary<SkillSlotKey, SkillData> skills { get; set; }
    public Player player { get; set; }
    public void StopCoroutine();
}

public interface IStopCoroutineS
{
    public Dictionary<SkillSlotKey, Skill> skills { get; set; }
    public Coroutine HoldSkillCoroutine { get; set; }
    public Player player { get; set; }
    public void StopCoroutine();
}

