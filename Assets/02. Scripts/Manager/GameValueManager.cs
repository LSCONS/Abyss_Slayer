using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameValueManager : Singleton<GameValueManager>
{
    [field: SerializeField] public int AddSkillPointValue { get; private set; } = 1;
    [field: SerializeField] public int AddStatusPointValue { get; private set; } = 1;
    [field: SerializeField] public int MaxBossCount { get; private set; } = 2;
    [field: SerializeField] public int CurrentStageIndex { get; private set; } = 0;

    public void NextStageIndex()
    {
        CurrentStageIndex++;
    }

    public void ResetStageIndex()
    {
        CurrentStageIndex = 0;
    }
}
