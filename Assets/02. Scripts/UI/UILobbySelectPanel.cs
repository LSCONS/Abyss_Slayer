using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.Unicode;
public enum Level
{
    Easy,
    Normal,
    Hard
}

public class UILobbySelectPanel : UIPermanent
{
    [field: SerializeField] private Button BtnLevelDown { get; set; }
    [field: SerializeField] private Button BtnLevelUp { get; set; }   // 오른쪽 버튼 (난이도 올림) 이거 근데 서클처럼 돌아갈 수 있게?
    [field: SerializeField] private Button BtnStartGame { get; set; }
    [field: SerializeField] private Button BtnExitGame { get; set; }

    [field: SerializeField] private TextMeshProUGUI TextLevelTitle { get; set; } // 난이도 제목
    [field: SerializeField] private TextMeshProUGUI TextLevelDesc { get; set; }  // 난이도 설명
    [field: SerializeField] private TextMeshProUGUI TextStartGame { get; set; } //시작 버튼 텍스트

    private Level currentLevel = Level.Easy;

    private void Awake()
    {
        UpdateUI();

        BtnLevelDown.onClick.AddListener(() => ChangeLevel(-1));
        BtnLevelUp.onClick.AddListener(() => ChangeLevel(1));
        BtnExitGame.onClick.AddListener(ServerManager.Instance.ExitRoom);
        ServerManager.Instance.LobbySelectPanel = this;
    }

    private void OnDisable()
    {
        BtnStartGame.onClick.RemoveAllListeners();
    }


    /// <summary>
    /// 순환함. 2되면 hard 0 되면 easy로 순환됨
    /// </summary>
    /// <param name="direction"></param>
    private void ChangeLevel(int direction)
    {
        int total = System.Enum.GetValues(typeof(Level)).Length;
        int newIndex = ((int)currentLevel + direction + total) % total;
        currentLevel = (Level)newIndex;

        UpdateUI(); // 값이 바뀌니까 다시 업데이트해야됨
    }


    /// <summary>
    /// 게임시작 버튼을 눌렀을 경우 실행할 메서드
    /// </summary>
    private void StartGame()
    {
        if (RunnerManager.Instance.GetRunner().IsServer)
        {
            ServerManager.Instance.InstantiatePlayer();
            ServerManager.Instance.ThisPlayerData.Rpc_MoveScene(ESceneName.Rest);
            RunnerManager.Instance.GetRunner().SessionInfo.IsOpen = false;
        }
    }


    /// <summary>
    /// 준비하기 버튼을 눌렀을 경우 실행할 메서드
    /// </summary>
    private void ReadyGame()
    {
        ServerManager.Instance.ThisPlayerData.Rpc_ClickReadyBtn();
    }


    /// <summary>
    /// 방을 만들었을 때 실행할 메서드
    /// </summary>
    public void SetServerInit()
    {
        TextStartGame.text = "시작하기";
        BtnStartGame.interactable = false;
        BtnStartGame.onClick.AddListener(StartGame);
    }


    /// <summary>
    /// 방에 입장했을 때 실행할 메서드
    /// </summary>
    public void SetClientInit()
    {
        TextStartGame.text = "준비하기";
        BtnStartGame.interactable = true;
        BtnStartGame.onClick.AddListener(ReadyGame);
    }


    /// <summary>
    /// 모든 플레이어 데이터에서 준비 상태를 확인하고 준비가 완료되었으면 시작 버튼을 활성화 해주는 메서드
    /// </summary>
    public void CheckAllPlayerIsReady()
    {
        foreach (NetworkData data in ServerManager.Instance.DictRefToNetData.Values)
        {
            if (!(data.IsReady))
            {
                BtnStartGame.interactable = false;
                return;
            }
        }
        BtnStartGame.interactable = true;
    }


    private void UpdateUI()
    {
        TextLevelTitle.text = currentLevel.ToString();

        switch (currentLevel)
        {
            case Level.Easy:
                TextLevelTitle.text = "난이도: 쉬움";
                TextLevelDesc.text = "쉬운 난이도입니다.\n클리어에 실패해도 재도전 가능합니다.";
                break;
            case Level.Normal:
                TextLevelTitle.text = "난이도: 보통";
                TextLevelDesc.text = "기본 난이도입니다.\n이정도는 기본이죠?";
                break;
            case Level.Hard:
                TextLevelTitle.text = "난이도: 어려움";
                TextLevelDesc.text = "님들이 깰 수 있을까요?";
                break;
        }
    }
}
