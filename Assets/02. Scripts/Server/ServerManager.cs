using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ServerManager : Singleton<ServerManager>, INetworkRunnerCallbacks
{
    [SerializeField]private string playerName = "Empty";
    public string PlayerName 
    {
        get   { return playerName; }
        set 
        { 
            playerName = value;
            ChangeNameAction?.Invoke();
        }
    }
    public byte[] PlayerNameBytes
    {
        get
        {
            byte[] temp = new byte[8];
            byte[] nameArray = System.Text.Encoding.UTF8.GetBytes(playerName);
            int arrayLenght = nameArray.Length;
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = (byte)((i < arrayLenght) ? nameArray[i] : 0);
            }
            return temp;
        }
    }
    public string RoomName { get; private set; } = "Empty";
    //플레이어의 이름이 바뀔 때 실행할 Action
    public Action ChangeNameAction { get; set; }
    public List<SessionInfo> CurrentSessionList { get; private set; }= new List<SessionInfo>();
    [SerializeField] private NetworkPrefabRef playerPrefab;

    //플레이어의 다양한 정보가 담겨있는 NetworkData를 딕셔너리로 저장함.
    public Dictionary<PlayerRef, NetworkData> DictPlayerDatas { get; private set; } =  new();

    //현재 접속한 플레이어의 정보를 담고 있는 PlayerRef
    public PlayerRef ThisPlayerRef { get; set; }

    [field: SerializeField] public NetworkData DataPrefab { get; private set; }

    //플레이어의 입력을 담당할 인풋
    public PlayerInput LocalInput { get; private set; }
    //방에 참가 할 수 있는 최대 인원 수
    public int MaxHeadCount { get; private set; } = 5;
    public UIChatController ChattingTextController { get; set; }
    public UILobbyMainPanel LobbyMainPanel { get; set; }
    public UILobbySelectPanel LobbySelectPanel { get; set; }
    public UIRoomSearch RoomSearch { get; set; }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public async void CreateRoom(string roomName)
    {
        LobbySelectPanel.SetServerInit();
        ChattingTextController.TextChattingRecord.text = "";//채팅 초기화
        RoomName = roomName;
        LobbyMainPanel.ChangeRoomText();
        var runner = RunnerManager.Instance.GetRunner();
        runner.ProvideInput = true;

        GameFlowManager.Instance.ClientSceneLoad(EGameState.Lobby);

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomName,
            Scene = scene,
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });
    }

    //public async void CreateRoom(string roomName)
    //{
    //    ChattingTextController.TextChattingRecord.text = "";//채팅 초기화
    //    RoomName = roomName;
    //    LobbyMainPanel.ChangeRoomText();
    //    var runner = RunnerManager.Instance.GetRunner();
    //    runner.ProvideInput = true;

    //    LobbySelectPanel.SetServerInit();

    //    await runner.StartGame(new StartGameArgs()
    //    {
    //        GameMode = GameMode.Host,
    //        SessionName = roomName,
    //        SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
    //    });
    //}

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
        ChattingTextController.TextChattingRecord.text = "";//채팅 초기화
        RoomName = info.Name;
        LobbyMainPanel.ChangeRoomText();
        var runner = RunnerManager.Instance.GetRunner();
        runner.ProvideInput = true;
        GameFlowManager.Instance.ClientSceneLoad(EGameState.Lobby);

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = info.Name,
            Scene = scene,
            SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        LobbySelectPanel.SetClientInit();
    }

    //public async void JoinRoom(SessionInfo info)
    //{
    //    ChattingTextController.TextChattingRecord.text = "";//채팅 초기화
    //    RoomName = info.Name;
    //    LobbyMainPanel.ChangeRoomText();
    //    var runner = RunnerManager.Instance.GetRunner();
    //    runner.ProvideInput = true;

    //    LobbySelectPanel.SetClientInit();

    //    await runner.StartGame(new StartGameArgs()
    //    {
    //        GameMode = GameMode.Client,
    //        SessionName = info.Name,
    //        SceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
    //    });
    //}

    /// <summary>
    /// 방을 떠날 때 사용 할 메서드
    /// </summary>
    public async void ExitRoom()
    {
        var runner = RunnerManager.Instance.GetRunner();
        await runner.Shutdown();
        Destroy(runner.gameObject);
        await Task.Yield();
        await ConnectRoomSearch();
        GameFlowManager.Instance.ClientSceneLoad(EGameState.Start);
    }

    /// <summary>
    /// 방을 찾기 위해서는 이 메서드를 호출해야함.
    /// </summary>
    public async Task ConnectRoomSearch()
    {
        var runner = RunnerManager.Instance.GetRunner();
        runner.AddCallbacks(this);
        runner.ProvideInput = false;
        await runner.JoinSessionLobby(SessionLobby.ClientServer);
    }


    public void OnConnectedToServer(NetworkRunner runner){}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        ExitRoom();
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
        if(runner.LocalPlayer == player) ThisPlayerRef = player;

        // 호스트만 실행
        if (!runner.IsServer) return;

        // Spawn: 네 번째 인자로 player를 넘기면 이 오브젝트에
        // 그 player가 Input Authority를 갖도록 자동 설정됩니다.
        runner.Spawn(
            DataPrefab,           // NetworkPrefabRef
            Vector3.zero,         // 초기 위치
            Quaternion.identity,  // 초기 회전
            player,               // ★ Input Authority 지정
            (r, spawnedObj) => {
                // ─── 여기서 딱 한 번만 초기화 ───
                var data = spawnedObj.GetComponent<NetworkData>();

                // 1) 이 오브젝트가 누구의 데이터인지 기록
                data.PlayerDataRef = player;
                data.IsServer = (player == runner.LocalPlayer);
                data.IsReady = (player == runner.LocalPlayer);
            }
        );
        LobbySelectPanel.CheckAllPlayerIsReady();
    }

    //플레이어가 나갔을 때 다른 플레이어들에게 실행되는 메서드. runner.SessionInfo는 유효하지만 해당 세션의 인원수는 감소된 형태로 적용된다.
    //나간 플레이어는 해당 메서드를 실행시키지 못한다.
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //누군가 방에서 나갔을 때 서버가 해당 플레이어의 데이터를 삭제하고 모든 플레이어에게 업데이트 하는 메서드
        if(runner.IsServer)
        {
            DictPlayerDatas[player].IsReady = false;
            runner.Despawn(DictPlayerDatas[player].GetComponent<NetworkObject>());
            LobbySelectPanel.CheckAllPlayerIsReady();
        }
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
        RoomSearch.UpdateRoomList();
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



