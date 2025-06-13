using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbySelectPanel : UIPermanent
{
    [field: SerializeField] private Button BtnLevelDown { get; set; }
    [field: SerializeField] private Button BtnLevelUp { get; set; }   // 오른쪽 버튼 (난이도 올림) 이거 근데 서클처럼 돌아갈 수 있게?
    [field: SerializeField] private Button BtnStartGame { get; set; }
    [field: SerializeField] private Button BtnExitGame { get; set; }

    [field: SerializeField] private TextMeshProUGUI TextLevelTitle { get; set; } // 난이도 제목
    [field: SerializeField] private TextMeshProUGUI TextLevelDesc { get; set; }  // 난이도 설명
    [field: SerializeField] private TextMeshProUGUI TextStartGame { get; set; } //시작 버튼 텍스트

    private EGameLevel currentLevel => ManagerHub.Instance.GameValueManager.EGameLevel;

    private void Awake()
    {
#if AllMethodDebug
        Debug.Log("Awake");
#endif
        UpdateUI((int)currentLevel);
        BtnLevelDown.onClick.AddListener(() => ChangeLevel(-1));
        BtnLevelUp.onClick.AddListener(() => ChangeLevel(1));
        BtnExitGame.onClick.AddListener(ManagerHub.Instance.ServerManager.ExitRoom);
        BtnExitGame.onClick.AddListener(() => ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick));
        ManagerHub.Instance.UIConnectManager.UILobbySelectPanel = this;
    }

    private void OnDisable()
    {
#if AllMethodDebug
        Debug.Log("OnDisable");
#endif
        BtnStartGame.onClick.RemoveAllListeners();
    }

    //나중에 로비 씬 입장할 때 해당 메서드 호출하게 해야 함. 꼭 서버 접속 후 호출
    public void JoinRoom()
    {
#if AllMethodDebug
        Debug.Log("JoinRoom");
#endif
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if(runner.IsServer)
        {
            SetActiveButton((int)currentLevel);
        }
        else
        {
            BtnLevelDown.interactable = false;
            BtnLevelUp.interactable = false;
        }
    }

    /// <summary>
    /// 순환함. 2되면 hard 0 되면 easy로 순환됨
    /// </summary>
    /// <param name="direction"></param>
    private void ChangeLevel(int direction)
    {
#if AllMethodDebug
        Debug.Log("ChangeLevel");
#endif
        ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
        int total = System.Enum.GetValues(typeof(EGameLevel)).Length - 1;
        int newIndex = (int)currentLevel + direction;

        SetActiveButton(newIndex);
        
        ManagerHub.Instance.GameValueManager.SetEGameLevel(newIndex);
        //RPC로 모든 플레이어에게 데이터 공유
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_LobbySelectLevelUpdateUI(newIndex);
    }


    /// <summary>
    /// 난이도 조절 버튼의 활성화 여부를 조절 하는 메서드
    /// </summary>
    /// <param name="EGameLevelInt"></param>
    private void SetActiveButton(int EGameLevelInt)
    {
#if AllMethodDebug
        Debug.Log("SetActiveButton");
#endif
        int total = System.Enum.GetValues(typeof(EGameLevel)).Length - 1;
        if (EGameLevelInt == 0)
        {
            BtnLevelDown.interactable = false;
        }
        else if (EGameLevelInt == total)
        {
            BtnLevelUp.interactable = false;
        }
        else
        {
            BtnLevelUp.interactable = true;
            BtnLevelDown.interactable = true;
        }
    }

    /// <summary>
    /// 게임시작 버튼을 눌렀을 경우 실행할 메서드
    /// </summary>
    private void StartGame()
    {
#if AllMethodDebug
        Debug.Log("StartGame");
#endif
        ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
        if (RunnerManager.Instance.GetRunner().IsServer)
        {
            //난이도 동기화도 해줌
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_SetGameLevel((int)ManagerHub.Instance.GameValueManager.EGameLevel);
            //새로운 플레이어 생성
            ManagerHub.Instance.ServerManager.InstantiatePlayer();
            //RestScene으로 이동
            ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_MoveScene(ESceneName.RestScene);
            //다른 플레이어가 못 들어오게 막음
            RunnerManager.Instance.GetRunner().SessionInfo.IsOpen = false;
        }

        // 파티 직업 정보 수집
        var playerClasses = new string[5];
        int index = 0;
        foreach (var data in ManagerHub.Instance.ServerManager.DictRefToNetData.Values)
        {
            if (index < 5)
            {
                playerClasses[index] = data.Class.ToString();
                index++;
            }
        }

        // 애널리틱스 전송
        GameStartAnalytics.SendStartUserInfo(
            ManagerHub.Instance.ServerManager.DictRefToNetData.Count,
            playerClasses[0], playerClasses[1], playerClasses[2], playerClasses[3], playerClasses[4]
        );
    }


    /// <summary>
    /// 준비하기 버튼을 눌렀을 경우 실행할 메서드
    /// </summary>
    private void ReadyGame()
    {
#if AllMethodDebug
        Debug.Log("ReadyGame");
#endif
        ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
        ManagerHub.Instance.ServerManager.ThisPlayerData.Rpc_ClickReadyBtn();
    }


    /// <summary>
    /// 방을 만들었을 때 실행할 메서드
    /// </summary>
    public void SetServerInit()
    {
#if AllMethodDebug
        Debug.Log("SetServerInit");
#endif
        TextStartGame.text = "시작하기";
        BtnStartGame.interactable = false;
        BtnStartGame.onClick.AddListener(StartGame);
    }


    /// <summary>
    /// 방에 입장했을 때 실행할 메서드
    /// </summary>
    public void SetClientInit()
    {
#if AllMethodDebug
        Debug.Log("SetClientInit");
#endif
        TextStartGame.text = "준비하기";
        BtnStartGame.interactable = true;
        BtnStartGame.onClick.AddListener(ReadyGame);
    }


    /// <summary>
    /// 모든 플레이어 데이터에서 준비 상태를 확인하고 준비가 완료되었으면 시작 버튼을 활성화 해주는 메서드
    /// </summary>
    public void CheckAllPlayerIsReady()
    {
#if AllMethodDebug
        Debug.Log("CheckAllPlayerIsReady");
#endif
        foreach (NetworkData data in ManagerHub.Instance.ServerManager.DictRefToNetData.Values)
        {
            if (data.IsServer) continue;

            if (!(data.IsReady))
            {
                BtnStartGame.interactable = false;
                return;
            }
        }
        BtnStartGame.interactable = true;
    }


    public void UpdateUI(int level)
    {
#if AllMethodDebug
        Debug.Log("UpdateUI");
#endif
        TextLevelTitle.text = ((EGameLevel)level).ToString();

        switch ((EGameLevel)level)
        {
            case EGameLevel.Easy:
                TextLevelTitle.text = "난이도: 쉬움";
                TextLevelDesc.text = "쉬운 난이도입니다.\n클리어에 실패해도 재도전 가능합니다.";
                break;
            case EGameLevel.Normal:
                TextLevelTitle.text = "난이도: 보통";
                TextLevelDesc.text = "기본 난이도입니다.\n클리어에 실패하면 처음부터 도전해야합니다.";
                break;
            case EGameLevel.Hard:
                TextLevelTitle.text = "난이도: 어려움";
                TextLevelDesc.text = "하드 난이도입니다.\n보스의 체력 및 플레이어의 피격 데미지가 늘어납니다.";
                break;
        }
    }
}
