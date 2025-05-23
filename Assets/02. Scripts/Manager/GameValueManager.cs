using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameValueManager : Singleton<GameValueManager>
{
    [field: Header("보스를 잡은 이후 추가할 스킬 포인트 양")]
    [field: SerializeField] public int AddSkillPointValue { get; private set; } = 1;
    [field: Header("보스를 잡은 이후 추가할 스텟 포인트 양")]
    [field: SerializeField] public int AddStatusPointValue { get; private set; } = 1;
    [field: Header("게임을 시작했을 때 지급 받을 스킬 포인트 양")]
    [field: SerializeField] public int InitSkillPointValue { get; private set; } = 1;
    [field: Header("게임을 시작했을 때 지급 받을 스텟 포인트 양")]
    [field: SerializeField] public int InitStatusPointValue { get; private set; } = 1;
    [field: Header("엔딩을 보기 위한 보스 스테이지 개수")]
    [field: SerializeField] public int MaxBossStageCount { get; private set; } = 2;
    [field: Header("현재 배틀 스테이지")]
    [field: SerializeField] public int CurrentStageIndex { get; private set; } = 0;
    [field: Header("보스 피격 컬러 유지 시간")]
    [field: SerializeField] public float OnDamageBossColorDuration { get; private set; } = 0.1f;
    [field: Header("플레이어 피격 컬러 유지 시간")]
    [field: SerializeField] public float OnDamagePlayerColorDuration { get; private set; } = 0.3f;
    [field: SerializeField] public float MinTypingSoundPitch { get; private set; } = 0.5f;
    [field: SerializeField] public float MaxTypingSoundPitch { get; private set; } = 1f;
    public EGameLevel EGameLevel { get; private set; } = EGameLevel.None;


    public void NextStageIndex()
    {
        CurrentStageIndex++;
    }


    public void ResetStageIndex()
    {
        CurrentStageIndex = 0;
    }
}

public enum EGameLevel
{
    None = 0,
    Easy,
    Normal,
    Hard,
}
