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
    [field: Header("하드모드에서 보스 최대 체력 증가 비율")]
    [field: SerializeField] public float HardBossMultipleHealth { get; private set; } = 1.3f;
    [field: Header("하드모드에서 플레이어 입는 피해 증가 비율")]
    [field: SerializeField] public float HardPlayerMultiypleOnDamage { get; private set; } = 1.3f;
    //플레이어가 선택한 스테이지 난이도
    public EGameLevel EGameLevel { get; private set; } = EGameLevel.Easy;
    //현재 배틀 스테이지
    public int CurrentStageIndex { get; private set; } = 0;
    public bool IsStageClear { get; private set; } = false;
    public string GameClearTime
    {
        get 
        {
            return PlayerPrefs.GetString("ClearTime", "하드 모드를 클리어 해주세요.");
        }
    }

    public void NextStageIndex()
    {
        if(IsStageClear)
        {
            CurrentStageIndex++;
            SetClearStage(false);
            if (RunnerManager.Instance.GetRunner().IsServer)
            {
                //TODO: 스킬포인트 주는 로직 변경 필요. 모든 플레이어들에게 RPC로 줘야함.
                foreach (Player player in ServerManager.Instance.DictRefToPlayer.Values)
                {
                    player.AddSkillPoint(AddSkillPointValue);
                    player.AddStatusPoint(AddStatusPointValue);
                }
            }
        }
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
