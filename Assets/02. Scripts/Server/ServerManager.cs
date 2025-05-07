using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ServerManager : Singleton<ServerManager>, INetworkRunnerCallbacks
{
    private string playerName = "Empty";
    public string PlayerName 
    {
        get 
        { 
            return playerName;
        }
        
        set 
        { 
            playerName = value;
            ChangeNameAction?.Invoke();
        }
    }
    //플레이어의 이름이 바뀔 때 실행할 Action
    public Action ChangeNameAction { get; set; }
    public List<SessionInfo> CurrentSessionList { get; private set; }= new List<SessionInfo>();

    public NetworkRunner _runner { get; private set; }
    [SerializeField] private NetworkPrefabRef playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    //플레이어의 입력을 담당할 인풋
    public PlayerInput LocalInput { get; private set; }
    
    //플레이어가 서버에 접속할 때마다 등록할 딕션너리
    public Dictionary<PlayerRef, NetworkRunner> PlayerDict { get; private set; }

    //플레이어가 준비되었는지 확인할 때 쓸 리스트1
    public HashSet<PlayerRef> PlayerReadyCount = new HashSet<PlayerRef>();

    //방에 참가 할 수 있는 최대 인원 수
    public int MaxHeadCount { get; private set; } = 5;

    //세션이 업데이트 될 때마다 실행할 Action 모음
    public Action ListUpdateAction { get; set; }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        if (_runner == null)
            _runner = gameObject.AddComponent<NetworkRunner>();
    }

    public void RpcSendReady(PlayerRef player)
    {
        PlayerReadyCount.Add(player);

        if(PlayerReadyCount.Count == PlayerDict.Count)
        {
            
        }
    }

    /// <summary>
    /// 호스트에게 서버의 씬을 바꾸라는 명령을 주는 메서드
    /// 근데 씬을 이름으로는 못 바꿈
    /// 
    /// </summary>
    public async void ServerSceneChange(string SceneName, LoadSceneMode mode)
    {
        await _runner.LoadScene(SceneRef.FromPath(SceneName), mode);
    }

    public async void CreateRoom(string roomName)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner.ProvideInput = true;

        GameFlowManager.Instance.ClientSceneLoad(EGameState.Lobby);

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    /// <summary>
    /// 같은 이름의 세션 이름의 방이 있는지 확인하는 메서드
    /// </summary>
    /// <returns>있다면 true, 없다면 false</returns>
    public bool CheckSameRoomName(string roomName)
    {
        foreach(var session in CurrentSessionList)
        {
            if(session.Name == roomName) return true;
        }
        return false;
    }


    /// <summary>
    /// 클라이언트가 방에 접속할 때 실행할 메서드
    /// </summary>
    /// <param name="info"></param>
    public async void JoinRoom(SessionInfo info)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner.ProvideInput = true;

        GameFlowManager.Instance.ClientSceneLoad(EGameState.Lobby);

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = info.Name,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    /// <summary>
    /// 방을 찾기 위해서는 이 메서드를 호출해야함.
    /// </summary>
    public async Task ConnectRoomSearch()
    {
        if (_runner == null)
            _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = false;
        _runner.AddCallbacks(this);
        await _runner.JoinSessionLobby(SessionLobby.ClientServer);
    }

    public void OnConnectedToServer(NetworkRunner runner){}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (LocalInput == null) return;

        NetworkInputData inputData = new NetworkInputData
        {
            MoveDir = LocalInput.MoveDir,
            IsJump = LocalInput.IsJump,
            IsSkillA = LocalInput.IsSkillA,
            IsSkillD = LocalInput.IsSkillD,
            IsSkillS = LocalInput.IsSkillS,
            IsSkillX = LocalInput.IsSkillX,
            IsSkillZ = LocalInput.IsSkillZ,
        };

        input.Set(inputData);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){}
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

    }

    //플레이어가 나갔을 때 다른 플레이어들에게 실행되는 메서드. runner.SessionInfo는 유효하지만 해당 세션의 인원수는 감소된 형태로 적용된다.
    //나간 플레이어는 해당 메서드를 실행시키지 못한다.
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){}
    public void OnSceneLoadDone(NetworkRunner runner){}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("방이 무언가 변했나요?");
        //플레이어가 없는 터진 방은 보이지 않도록 함.
        List<SessionInfo> temp = new List<SessionInfo>();
        foreach(SessionInfo sessionInfo in sessionList)
        {
            Debug.Log("추가 할까요?");
            if(sessionInfo.PlayerCount == 0) continue;
            Debug.Log("temp에 추가 됐습니다!!");
            temp.Add(sessionInfo);
        }
        CurrentSessionList = temp;
        if (ListUpdateAction == null) Debug.Log("비어있었네요");
        ListUpdateAction?.Invoke();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
    public void RegisterLocalInput(PlayerInput input)
    {
        LocalInput = input;
    }
}

public struct NetworkInputData : INetworkInput
{
    // 이동 방향
    public Vector2 MoveDir;
    // 점프, 스킬 A/D/S/X/Z
    public NetworkBool IsJump;
    public NetworkBool IsSkillA;
    public NetworkBool IsSkillD;
    public NetworkBool IsSkillS;
    public NetworkBool IsSkillX;
    public NetworkBool IsSkillZ;
}



