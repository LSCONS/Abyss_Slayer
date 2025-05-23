using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AuidoClipDataGather", menuName = "Data/AudioClipData")]
public class AudioClipDataGather : ScriptableObject
{
    [field: SerializeField] public List<AudioClipEnumData> ListAudioClipEnumData { get; private set; } = new();
}

[Serializable]
public class AudioClipEnumData
{
    [field: SerializeField] public AudioClipData AudioClipData { get; private set; }
    [field: SerializeField] public EAudioClip EnumClip { get; private set; }
}

[Serializable]
public class AudioClipData
{
    [field: SerializeField] public AudioClip Audio { get; private set; }
    [field: SerializeField] public bool IsLoop { get; private set; } = false;
    [field: SerializeField] public float Pitch { get; private set; } = 1;
    [field: SerializeField] public float Volume { get; private set; } = 1;
}

// 추가할 요소는 각 번호대의 마지막에 추가하기
public enum EAudioClip
{
    None = 0,
    BGM_IntroScene = 100,
    BGM_StartScene = 101,
    BGM_LobbyScene = 102,
    BGM_RestScene = 103,
    BGM_BattleScene = 104,
    BGM_BattleScene_Last = 105,
    BGM_EndingScene = 106,

    SFX_PlayerJump = 201,
    SFX_PlayerDash = 202,
    SFX_KeyBoardTyping = 203,
    SFX_ButtonClick = 204,

    SFX_RogueSkill_X = 300,
    SFX_RogueSkill_A = 301,
    SFX_RogueSkill_S = 302,
    SFX_RogueSkill_D = 303,

    SFX_HealerSkill_X = 400,
    SFX_HealerSkill_A = 401,
    SFX_HealerSkill_S = 402,
    SFX_HealerSkill_D = 403,

    SFX_TankerSkill_X = 500,
    SFX_TankerSkill_A = 501,
    SFX_TankerSkill_S = 502,
    SFX_TankerSkill_D = 503,

    SFX_MageSkill_X = 600,
    SFX_MageSkill_A = 601,
    SFX_MageSkill_S = 602,
    SFX_MageSkill_D = 603,

    SFX_MagicalBlasderSkill_X = 700,
    SFX_MagicalBlasderSkill_A = 701,
    SFX_MagicalBlasderSkill_S = 702,
    SFX_MagicalBlasderSkill_D = 703,

    SFX_Fire = 800,
    SFX_Human_Atk_Sword1,
    SFX_Human_Atk_Sword2,
    SFX_Human_Atk_Sword3,
    SFX_Ice_Explosion1,
    SFX_Thunder2,
    SFX_Water2,
    SFX_Wind1,
    SFX_Earth2,
    SFX_Charge5,
    SFX_Poision,

    SFX_Claw = 900,
    SFX_Bite,
    SFX_Impact_Flesh,
    SFX_Slash,
    SFX_MissEvade,
    SFX_Block,
    SFX_Flee,
    SFX_Encounter,
    SFX_EnemyDeath,
    SFX_Flesh2,

    SFX_Heal2 = 1000,
    SFX_Atk_Buff,
    SFX_Def_Buff,
    SFX_Debuff1,
    SFX_Revive3,
    SFX_Absorb4,
    SFX_Sleep1,
    SFX_SpeedUp2,

    SFX_Deep_Whosh = 1100,
    SFX_Draw_Sword1,
    SFX_Explosion,
    SFX_Gameover2,
    SFX_Horror_Warning,
    SFX_Impact_Sound,
    SFX_Metal_Beaten,
    SFX_Retro_You_Lose,
    SFX_Boss_Attack1_14,
    SFX_Boss_Attack2_15,
    SFX_Skip_38509,
    SFX_Spill_Swish,
    SFX_Sword_260274,
    SFX_Woosh_230554,
    SFX_Woosh_255592,

}
