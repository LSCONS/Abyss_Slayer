using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BossPrefabData", menuName = "Data/BossPrefabData")]
public class BossPrefabDataGather : ScriptableObject
{
    [field: SerializeField] public List<BossPrefabData> ListBossPrefabData { get; private set; } = new();
}

[Serializable]
public class BossPrefabData
{
    [field: SerializeField] public NetworkObject BossObject { get; private set; }
    [field: SerializeField] public EBossStage BossStage { get; private set; }
}

public enum EBossStage
{
    Boss0 = 0,
    Boss1 = 1,





    Rest,
    None,
}
