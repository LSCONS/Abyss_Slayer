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


public enum EAudioClip
{
    None = 0,
    BGM_StartScene,
    BGM_LobbyScene,
    BGM_RestScene,
    BGM_BattleScene,

    SFX_ButtonClick,

    SFX_PlayerWalk,
    SFX_PlayerJump,
    SFX_PlayerDsah,

    SFX_RogueSkill_X,
    SFX_RogueSkill_A,
    SFX_RogueSkill_S,
    SFX_RogueSkill_D,

    SFX_HealerSkill_X,
    SFX_HealerSkill_A,
    SFX_HealerSkill_S,
    SFX_HealerSkill_D,

    SFX_TankerSkill_X,
    SFX_TankerSkill_A,
    SFX_TankerSkill_S,
    SFX_TankerSkill_D,

    SFX_MageSkill_X,
    SFX_MageSkill_A,
    SFX_MageSkill_S,
    SFX_MageSkill_D,

    SFX_MagicalBlasterSkill_X,
    SFX_MagicalBlasterSkill_A,
    SFX_MagicalBlasterSkill_S,
    SFX_MagicalBlasterSkill_D,
}
