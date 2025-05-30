using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BuffImageData", menuName = "Data/BuffImageData")]
public class BuffSpriteDataGather : ScriptableObject
{
    [field: SerializeField] public List<BuffSpriteData> ListBuffImageData { get; private set; } = new();
}

[Serializable]
public class BuffSpriteData
{
    [field: SerializeField] public Sprite BuffSprite { get; private set; }
    [field: SerializeField] public EBuffType EBuffType { get; private set; } = EBuffType.None;
}

public enum EBuffType
{
    None = 0,
    Stun,
    Weaken,
    Slow,
    RogueDoubleShot, //아처 더블 샷 버프
}
