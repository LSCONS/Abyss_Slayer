using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public static BasicSpawner Instance { get; private set; }
    private NetworkRunner _runner;
    [SerializeField] private NetworkPrefabRef playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private PlayerInput localInput;

    private void Awake()
    {
        Instance = this;
    }

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            ObjectProvider = new PooledNetworkObjectProvider(),
        });
    }

    private void OnGUI()
    {
        if(_runner == null)
        {
            if (GUI.Button(new Rect(0,0,200,40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if(GUI.Button(new Rect(0,40,200,40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

    public void OnConnectedToServer(NetworkRunner runner){}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason){}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (localInput == null) return;

        NetworkInputData inputData = new NetworkInputData
        {
            MoveDir = localInput.MoveDir,
            IsJump = localInput.IsJump,
            IsSkillA = localInput.IsSkillA,
            IsSkillD = localInput.IsSkillD,
            IsSkillS = localInput.IsSkillS,
            IsSkillX = localInput.IsSkillX,
            IsSkillZ = localInput.IsSkillZ,
        };

        input.Set(inputData);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
            spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if(spawnedCharacters.TryGetValue(player, out NetworkObject networkPlayerObject))
        {
            runner.Despawn(networkPlayerObject);
            spawnedCharacters.Remove(player);      //연결된 플레이어 삭제
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
    public void OnSceneLoadDone(NetworkRunner runner){}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
    public void RegisterLocalInput(PlayerInput input)
    {
        localInput = input;
    }
}
