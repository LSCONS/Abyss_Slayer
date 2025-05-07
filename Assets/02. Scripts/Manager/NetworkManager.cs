using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkManager : SingletonNetwork<NetworkManager>
{
    public Dictionary<PlayerRef, PlayerBattleData> DictPlayerData { get; private set; } = new();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }


    public void AddPlayerData(PlayerRef player)
    {
        DictPlayerData[player] = new PlayerBattleData();
    }

    public void RemovePlayerData(PlayerRef player)
    {
        DictPlayerData.Remove(player);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcUpdatePlayerData()
    {
        ;
    }
}

public class PlayerBattleData
{
    public NetworkObject PlayerObject {  get; set; }
    public CharacterClass PlayerClass { get; set; } = CharacterClass.Rogue;
    public PlayerStatusData PlayerStatus { get; set; }
    public CharacterSkillSet PlayerSkillSet { get; set; }
}
