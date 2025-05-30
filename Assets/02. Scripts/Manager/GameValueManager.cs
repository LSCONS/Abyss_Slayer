using Fusion;
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
    [field: Header("보스 피격 컬러 유지 시간")]
    [field: SerializeField] public float OnDamageBossColorDuration { get; private set; } = 0.1f;
    [field: Header("플레이어 피격 컬러 유지 시간")]
    [field: SerializeField] public float OnDamagePlayerColorDuration { get; private set; } = 0.3f;
    [field: Header("타이핑 소리 랜덤Pitch 최소 값")]
    [field: SerializeField] public float MinTypingSoundPitch { get; private set; } = 0.5f;
    [field: Header("타이핑 소리 랜덤Pitch 최대 값")]
    [field: SerializeField] public float MaxTypingSoundPitch { get; private set; } = 1f;
    [field: Header("클라이언트가 서버로부터 공유받은 위치를 이동하는 보간 속도")]
    [field: SerializeField] public float MoveSmoothObjectPositionForClientValue { get; private set; } = 50f;
    [field: Header("특정모드에서 보스 최대 체력 증가 비율")]
    [field: SerializeField] public float EasyBossHealthMultipleForLevel { get; private set; } = 0.7f;
    [field: SerializeField] public float NormalBossHealthMultipleForLevel { get; private set; } = 1f;
    [field: SerializeField] public float HardBossHealthMultipleForLevel { get; private set; } = 1.3f;
    [field: Header("특정모드에서 플레이어 입는 피해 증가 비율")]
    [field: SerializeField] public float EasyPlayerMultiypleOnDamage { get; private set; } = 0.7f;
    [field: SerializeField] public float NormalPlayerMultiypleOnDamage { get; private set; } = 1f;
    [field: SerializeField] public float HardPlayerMultiypleOnDamage { get; private set; } = 1.3f;
    [field: Header("특정모드에서 플레이어당 보스 체력 증가 비율")]
    [field: SerializeField] public float EasyBossHealthMultipleForPlayerCount { get; private set; } = 0.85f;
    [field: SerializeField] public float NormalBossHealthMultipleForPlayerCount { get; private set; } = 0.85f;
    [field: SerializeField] public float HardBossHealthMultipleForPlayerCount { get; private set; } = 0.85f;
    //플레이어가 선택한 스테이지 난이도
    public EGameLevel EGameLevel { get; private set; } = EGameLevel.Easy;
    //현재 배틀 스테이지
    public int CurrentStageIndex { get; private set; } = 0;
    public bool IsStageClear { get; private set; } = false;
    public string GameSoloClearTime
    {
        get 
        {
            return PlayerPrefs.GetString(PlayerPrefabData.SoloClearTime, $"[닉네임]\n해금 조건: 하드 모드 1인 클리어");
        }
    }

    public string GameMultiClearTime
    {
        get
        {
            return PlayerPrefs.GetString(PlayerPrefabData.MultiClearTime, $"[닉네임]\n해금 조건: 하드 모드 2인 이상 클리어");
        }
    }

    public void NextStageIndex()
    {
        if(IsStageClear)
        {
            CurrentStageIndex++;
            SetClearStage(false);
            NetworkRunner runner = RunnerManager.Instance.GetRunner();
            if (runner.IsServer)
            {
                ServerManager.Instance.ThisPlayerData.Rpc_AddSkillPoint();
            }
        }
    }


    /// <summary>
    /// 난이도에 따른 보스의 최대 체력 증감율 반환
    /// </summary>
    /// <returns></returns>
    public float GetBossHealthMultipleForLevelValue()
    {
        return EGameLevel switch
        {
            EGameLevel.Easy => EasyBossHealthMultipleForLevel,
            EGameLevel.Normal => NormalBossHealthMultipleForLevel,
            EGameLevel.Hard => HardBossHealthMultipleForLevel,
            _ => 0f
        };
    }


    /// <summary>
    /// 난이도와 플레이어 수에 따른 보스 체력 증감율을 반환
    /// </summary>
    /// <returns></returns>
    public float GetBossHealthMultipleForPlayerCountValue()
    {
        return EGameLevel switch
        {
            EGameLevel.Easy => EasyBossHealthMultipleForPlayerCount,
            EGameLevel.Normal => NormalBossHealthMultipleForPlayerCount,
            EGameLevel.Hard => HardBossHealthMultipleForPlayerCount,
            _ => 0f
        };
    }


    /// <summary>
    /// 난이도에 따른 플레이어의 입는 피해 비율을 반환
    /// </summary>
    /// <returns></returns>
    public float GetPlayerMultiypleOnDamageValue()
    {
        return EGameLevel switch
        {
            EGameLevel.Easy => EasyPlayerMultiypleOnDamage,
            EGameLevel.Normal => NormalPlayerMultiypleOnDamage,
            EGameLevel.Hard => HardPlayerMultiypleOnDamage,
            _ => 0f
        };
    }


    public void ResetStageIndex()
    {
        CurrentStageIndex = 0;
    }


    public void SetEGameLevel(int levelInt)
    {
        EGameLevel = (EGameLevel)levelInt;
    }

    public void SetClearStage(bool isClear)
    {
        IsStageClear = isClear;
    }
}

public enum EGameLevel
{
    Easy = 0,
    Normal,
    Hard,
}
