using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BossPrefabData", menuName = "Data/BossPrefabData")]
public class BossPrefabData : ScriptableObject
{
    [field: SerializeField] public List<BossPrefabStruct> ListBossPrefabStruct { get; private set; } = new();
}

[Serializable]
public struct BossPrefabStruct
{
    [field: SerializeField] public NetworkObject BossObject { get; private set; }
    [field: SerializeField] public EBossStage BossStage { get; private set; }
}

public enum EBossStage
{
    None = 0,
    Rest,
    Boss0,
    Boss1,
}
