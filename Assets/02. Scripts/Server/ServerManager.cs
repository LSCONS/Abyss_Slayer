using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ServerManager : INetworkRunnerCallbacks
{
    public UIConnectManager UIConnectManager => ManagerHub.Instance.UIConnectManager;
    public Transform TrManagerHub => ManagerHub.Instance.transform;
    public Dictionary<PlayerRef, Player> DictRefToPlayer { get; private set; } = new();
    public int BossCount { get; private set; } = 0;
    public string PlayerName { get; set; } = "Empty";
    public string ServerVersion => Application.version;
    public byte[] PlayerNameBytes
    {
        get
        {
            byte[] temp = new byte[24];
            byte[] nameArray = System.Text.Encoding.UTF8.GetBytes(PlayerName);
            int arrayLenght = nameArray.Length;
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = (byte)((i < arrayLenght) ? nameArray[i] : 0);
            }
            return temp;
        }
    }
    [field: SerializeField] public List<NetworkObject> BossObject { get; private set; }
    public string StrHostVersionKey { get; private set; } = "HostVersion";
    public string RoomName { get; private set; } = "Empty";
    //플레이어의 이름이 바뀔 때 실행할 Action
    public Action ChangeNameAction { get; set; }
    public List<SessionInfo> CurrentSessionList { get; private set; } = new();
    public List<SessionInfo> AllSessionList { get; private set; } = new();

    //플레이어의 다양한 정보가 담겨있는 NetworkData를 딕셔너리로 저장함.
    public Dictionary<PlayerRef, NetworkData> DictRefToNetData { get; private set; } = new();
    //현재 접속한 플레이어의 정보를 담고 있는 PlayerRef
    public PlayerRef ThisPlayerRef { get; set; }
    //현재 접속한 플레이어의 정보를 반환하는 변수. 찾지 못하면 null반환
    public NetworkData ThisPlayerData
        => DictRefToNetData.TryGetValue(ThisPlayerRef, out var data) ? data : null;
    public Player ThisPlayer 
        => DictRefToPlayer.TryGetValue(ThisPlayerRef, out var player) ? player : null;
    public int MaxHeadCount { get; private set; } = 5;
    public bool IsServer { get; private set; } = false;
    public Fireworks fireworks { get; set; }
    public InitSupporter InitSupporter { get; set; }
    public PoolManager PoolManager { get; set; }
    public Vector3 Vec3PlayerBattlePosition { get; private set; } = new Vector3(-18, 1.5f, 0);
    public Vector3 Vec3PlayerRestPosition { get; private set; } = new Vector3(-5, 1.5f, 0);
    public Action<bool> IsAllReadyAction { get; set; }
    public PlayerInput PlayerInput 
    {
        get
        {
            if (playerInput != null) return playerInput;
            return playerInput = TrManagerHub.GetComponent<PlayerInput>() ?? TrManagerHub.gameObject.AddComponent<PlayerInput>();
        }
    }
    private PlayerInput playerInput;
    public Boss Boss { get; set; } = null;


    /// <summary>
    /// 모든 플레이어가 준비가 되었는지 서버에서 확인하는 메서드
    /// </summary>
    /// <returns></returns>
    public bool CheckAllPlayerIsReadyInServer()
    {
#if AllMethodDebug
        Debug.Log("CheckAllPlayerIsReadyInServer");
#endif
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (!(runner.IsServer)) return false;
        foreach (NetworkData data in DictRefToNetData.Values)
        {
            if (data == ThisPlayerData) continue;
            if (!(data.IsReady)) return false;
        }
        return true;
    }


    /// <summary>
    /// 모든 플레이어의 준비 상태를 false로 초기화하는 메서드
    /// </summary>
    public void AllPlayerIsReadyFalse()
    {
#if AllMethodDebug
        Debug.Log("AllPlayerIsReadyFalse");
#endif
        if (RunnerManager.Instance.GetRunner().IsServer)
        {
            foreach(NetworkData data in DictRefToNetData.Values)
            {
                data.IsReady = false;
            }
        }
    }


    public List<Player> GetActiveTruePlayer()
    {
        List<Player> list = new List<Player>();
        foreach (Player player in DictRefToPlayer.Values)
        {
            if (player.PlayerData.PlayerStatusData.IsDead) continue;
            list.Add(player);
        }
        return list;
    }


    public async Task WaitForPlayerState()
    {
        while (UIConnectManager.UIPlayerState == null)
        {
            await Task.Delay(100);
        }
        return;
    }


    public async Task WaitForHairCrossObject()
    {
#if AllMethodDebug
        Debug.Log("WaitForHairCrossObject");
#endif
        await WaitForPoolManager();
        while (PoolManager.CrossHairObject == null)
        {
            PoolManager.CrossHairObject = GameObject.FindAnyObjectByType<NetworkObjectFollowServer>();
            await Task.Delay(100);
        }
        return;
    }


    public async Task WaitForPoolManager()
    {
#if AllMethodDebug
        Debug.Log("WaitForPoolManager");
#endif
        while (PoolManager == null)
        {
            PoolManager = GameObject.FindAnyObjectByType<PoolManager>();
            await Task.Delay(100);
        }
        return;
    }


    public async Task WaitforBossSpawn()
    {
#if AllMethodDebug
        Debug.Log("WaitforBossSpawn");
#endif
        int LoopNum = 0;
        while (Boss == null || LoopNum > 10)
        {
            await Task.Delay(100);
            LoopNum++;
        }
        return;
    }


    /// <summary>
    /// 모든 플레이어의 준비가 true가 될 때까지 대기하는 메서드
    /// </summary>
    /// <returns></returns>
    public async Task WaitForAllPlayerIsReadyTrue()
    {
#if AllMethodDebug
        Debug.Log("WaitForAllPlayerIsReadyTrue");
#endif
        while (true)
        {
            int sessionPlayerCount = RunnerManager.Instance.GetRunner().SessionInfo.PlayerCount;
            int isReadyPlayerCount = 0;
            foreach (NetworkData data in DictRefToNetData.Values)
            {
                if(data.IsReady) isReadyPlayerCount++;
            }

            await Task.Yield();
            if(sessionPlayerCount == isReadyPlayerCount)
            {
                break;
            }
        }
        return;
    }


    /// <summary>
    /// 모든 플레이어의 준비가 true가 될 때까지 대기하는 메서드
    /// </summary>
    /// <returns></returns>
    public async Task WaitForAllPlayerIsReadyFalse()
    {
#if AllMethodDebug
        Debug.Log("WaitForAllPlayerIsReadyFalse");
#endif
        while (true)
        {
            int sessionPlayerCount = RunnerManager.Instance.GetRunner().SessionInfo.PlayerCount;
            int isReadyPlayerCount = 0;
            foreach (NetworkData data in DictRefToNetData.Values)
            {
                if (!(data.IsReady)) isReadyPlayerCount++;
            }

            await Task.Yield();
            if (sessionPlayerCount == isReadyPlayerCount)
            {
                break;
            }
        }
        return;
    }


    /// <summary>
    /// 자신의 Player에 값이 들어올 때까지 대기하고 반환하는 메서드
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<Player> WaitForThisPlayerAsync()
    {
#if AllMethodDebug
        Debug.Log("WaitForThisPlayerAsync");
#endif
        Player player;
        while ((player = ThisPlayer) == null) 
        {
            await Task.Delay(100);
        }
        return player;
    }

    public async Task WaitForDespawnBossAsync(CancellationToken ct = default)
    {
        while(Boss != null)
        {
            await Task.Delay(100);
        }
        return;
    }


    public async Task WaitForThisPlayerDataAsync(CancellationToken ct = default)
    {
#if AllMethodDebug
        Debug.Log("WaitForThisPlayerDataAsync");
#endif
        NetworkData data = null;
        while((data = ThisPlayerData) == null)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(100);
        }
        return;
    }


    /// <summary>
    /// 모든 플레어의 Player에 값이 들어올 때까지 대기하는 메서드
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task WaitForAllPlayerLoadingAsync(CancellationToken ct = default)
    {
#if AllMethodDebug
        Debug.Log("WaitForAllPlayerLoadingAsync");
#endif
        while (DictRefToNetData.Count != DictRefToPlayer.Count)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(100);
        }
        return;
    }


    /// <summary>
    /// 플레이어의 수 만큼 Player를 생성하는 메서드
    /// </summary>
    public void InstantiatePlayer()
    {
#if AllMethodDebug
        Debug.Log("InstantiatePlayer");
#endif
        NetworkRunner runner = RunnerManager.Instance.GetRunner();

        Vector3 tempVec3 = Vec3PlayerBattlePosition;
        foreach (PlayerRef playerRef in DictRefToNetData.Keys)
        {
            var spawnResult = runner.Spawn
            (
                ManagerHub.Instance.DataManager.PlayerPrefab,
                tempVec3,
                Quaternion.identity,
                playerRef,
                (runner, obj) =>
                {
                    Player player = obj.GetComponent<Player>();
                    player.PlayerRef = playerRef;
                    player.PlayerPosition = tempVec3;
                }
            );
            tempVec3 += Vector3.right;
        }
    }


    /// <summary>
    /// 호스트가 되어 방을 만드는 메서드
    /// </summary>
    /// <param name="roomName"></param>
    public void CreateRoom(string roomName)
    {
#if AllMethodDebug
        Debug.Log("CreateRoom");
#endif
        UIConnectManager.UIChatController.TextChattingRecord.text = "";//채팅 초기화
        RoomName = roomName;
        UIConnectManager.UILobbyMainPanel.ChangeRoomText();
        var runner = RunnerManager.Instance.GetRunner();
        runner.ProvideInput = true;
        IsServer = true;
        ManagerHub.Instance.GameFlowManager.ClientSceneLoad(ESceneName.LobbyScene);
    }


    /// <summary>
    /// 같은 이름의 세션 이름의 방이 있는지 확인하는 메서드
    /// </summary>
    /// <returns>있다면 true, 없다면 false</returns>
    public bool CheckSameRoomName(string roomName)
    {
#if AllMethodDebug
        Debug.Log("CheckSameRoomName");
#endif
        foreach (var session in CurrentSessionList)
        {
            if (session.Name == roomName) return true;
        }
        return false;
    }


    /// <summary>
    /// 클라이언트가 방에 접속할 때 실행할 메서드
    /// </summary>
    /// <param name="info"></param>
    public void JoinRoom(SessionInfo info)
    {
#if AllMethodDebug
        Debug.Log("JoinRoom");
        Debug.Log($"RoomName = {RoomName}");
#endif
        UIConnectManager.UIChatController.TextChattingRecord.text = "";//채팅 초기화
        RoomName = info.Name;
        UIConnectManager.UILobbyMainPanel.ChangeRoomText();
        var runner = RunnerManager.Instance.GetRunner();
        runner.ProvideInput = true;
        IsServer = false;
        ManagerHub.Instance.GameFlowManager.ClientSceneLoad(ESceneName.LobbyScene);
    }

    public async Task InitTutorial()
    {
#if AllMethodDebug
        Debug.Log("InitTutorial");
#endif
        var runner = RunnerManager.Instance.GetRunner();
        Debug.Log("방 만들기 시작");
        var temp = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = $"{PlayerName}-Tutorial-",
            Scene = SceneRef.FromIndex((int)ESceneName.TutorialScene),
            AuthValues = new Fusion.Photon.Realtime.AuthenticationValues(PlayerName)
        });
        await temp;
        return;
    }

    public async Task InitHost()
    {
#if AllMethodDebug
        Debug.Log("InitHost");
        Debug.Log($"RoomName = {RoomName}");
#endif
        var runner = RunnerManager.Instance.GetRunner();
        var temp = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = RoomName,
            SessionProperties = new Dictionary<string, SessionProperty>()
                {
                    { StrHostVersionKey, ServerVersion }
                },
            Scene = SceneRef.FromIndex((int)ESceneName.LobbyScene),
            AuthValues = new Fusion.Photon.Realtime.AuthenticationValues(PlayerName)
            
        });
        await temp;
        UIConnectManager.UILobbySelectPanel.SetServerInit();
        return;
    }

    public async Task InitClient()
    {
#if AllMethodDebug
        Debug.Log("InitClient");
#endif
        var runner = RunnerManager.Instance.GetRunner();
        var temp = runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = RoomName,
            Scene = SceneRef.FromIndex((int)ESceneName.LobbyScene),
            AuthValues = new Fusion.Photon.Realtime.AuthenticationValues(PlayerName)
        });
        await temp;
        UIConnectManager.UILobbySelectPanel.SetClientInit();
        return;
    }


    /// <summary>
    /// 방을 떠날 때 사용 할 메서드
    /// </summary>
    public async void ExitRoom()
    {
#if AllMethodDebug
        Debug.Log("ExitRoom");
#endif
        var runner = RunnerManager.Instance.GetRunner();
        await runner.Shutdown();
        try
        {
            GameObject.Destroy(runner.gameObject);
        }
        catch { }
        await Task.Yield();
        ManagerHub.Instance.GameFlowManager.ClientSceneLoad(ESceneName.StartScene);
        await ConnectRoomSearch();
        Scene temp = SceneManager.GetSceneByName("FusionSceneManager_TempEmptyScene");
        if (temp.IsValid() && temp.isLoaded)
        {
            // 비동기 언로드 실행
            AsyncOperation op = SceneManager.UnloadSceneAsync(temp);
        }
    }


    /// <summary>
    /// 방을 찾기 위해서는 이 메서드를 호출해야함.
    /// </summary>
    public async Task ConnectRoomSearch()
    {
#if AllMethodDebug
        Debug.Log("ConnectRoomSearch");
#endif
        var runner = RunnerManager.Instance.GetRunner();
        runner.AddCallbacks(this);
        runner.ProvideInput = false;
        var temp = runner.JoinSessionLobby(SessionLobby.ClientServer);
        await temp;
        IsServer = false;
        return;
    }


    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        ExitRoom();
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData inputData = new NetworkInputData
        {
            MoveDir = PlayerInput.MoveDir,
            IsJump = PlayerInput.IsJump,
            IsSkillA = PlayerInput.IsSkillA,
            IsSkillD = PlayerInput.IsSkillD,
            IsSkillS = PlayerInput.IsSkillS,
            IsSkillX = PlayerInput.IsSkillX,
            IsSkillZ = PlayerInput.IsSkillZ,
        };

        input.Set(inputData);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.LocalPlayer == player) ThisPlayerRef = player;

        // 호스트만 실행
        if (!runner.IsServer) return;

        runner.Spawn(
            ManagerHub.Instance.DataManager.PlayerNetworkDataPrefab,           // NetworkPrefabRef
            Vector3.zero,         // 초기 위치
            Quaternion.identity,  // 초기 회전
            player,               // Input Authority 지정
            (r, spawnedObj) =>
            {
                var data = spawnedObj.GetComponent<NetworkData>();
                data.PlayerDataRef = player;
                data.IsServer = (player == runner.LocalPlayer);
                data.IsReady = (player == runner.LocalPlayer);
                data.IntPlayerClass = (int)CharacterClass.Rogue;
                data.FaceKey = 1;
                data.SkinKey = 1;
                data.HairStyleKey = 1;
            }
        );
        ManagerHub.Instance.UIConnectManager.UILobbySelectPanel.CheckAllPlayerIsReady();
    }

    //플레이어가 나갔을 때 다른 플레이어들에게 실행되는 메서드. runner.SessionInfo는 유효하지만 해당 세션의 인원수는 감소된 형태로 적용된다.
    //나간 플레이어는 해당 메서드를 실행시키지 못한다.
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        //누군가 방에서 나갔을 때 서버가 해당 플레이어의 데이터를 삭제하고 모든 플레이어에게 업데이트 하는 메서드
        if(DictRefToNetData.TryGetValue(playerRef, out var networkData))
        {
            if (runner.IsServer)
            {
                runner.Despawn(networkData.GetComponent<NetworkObject>());
            }
            DictRefToNetData.Remove(playerRef);
        }

        if (DictRefToPlayer.TryGetValue(playerRef, out var player))
        {
            if (runner.IsServer)
            {
                runner.Despawn(player.GetComponent<NetworkObject>());
            }
            DictRefToPlayer.Remove(playerRef);
        }


        if (runner.IsServer)
        {
            UIConnectManager.UILobbySelectPanel?.CheckAllPlayerIsReady();
            bool isAllReday = CheckAllPlayerIsReadyInServer();
            IsAllReadyAction?.Invoke(isAllReday);
        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        CurrentSessionList = sessionList;
        try
        {
            UIConnectManager.UIRoomSearch.UpdateRoomList();
        }
        catch { }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void RegisterLocalInput(PlayerInput input)
    {
        //LocalInput = input;
    }
}

public struct NetworkInputData : INetworkInput
{
    // 이동 방향
    public Vector2 MoveDir { get; set; }
    // 점프, 스킬 A/D/S/X/Z
    public NetworkBool IsJump { get; set; }
    public NetworkBool IsSkillA { get; set; }
    public NetworkBool IsSkillD { get; set; }
    public NetworkBool IsSkillS { get; set; }
    public NetworkBool IsSkillX { get; set; }
    public NetworkBool IsSkillZ { get; set; }
}



