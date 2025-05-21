using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIReadyBossStage : UIButton
{
    [field: SerializeField] public Button BtnReadyOrStart { get;private set; }
    [field: SerializeField] public TextMeshProUGUI TextBtnReadyOrStart { get;private set;}
    [field: SerializeField] public Image ImgBossIcon { get;private set; }
    [field: SerializeField] public Image ImgBtnColor { get;private set; }
    public string ServerText { get; private set; } = "게임 시작하기";
    public string ClientText { get; private set; } = "게임 준비하기";
    private bool IsReady { get; set; } = false;
    [field: SerializeField] public Color OnReadyColor { get; private set; }
    [field: SerializeField] public Color OffReadyColor { get; private set; }

    private void Awake()
    {
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        TextBtnReadyOrStart.text = runner.IsServer ? ServerText : ClientText;
        BtnReadyOrStart.interactable = runner.IsServer ? false : true;
        if (runner.IsServer)
        {
            BtnReadyOrStart.onClick.AddListener(ClickStartButton);
        }
        else
        {
            BtnReadyOrStart.onClick.AddListener(ClickReadyButton);
        }
    }

    public override void Init()
    {
        base.Init();
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            foreach(var data in ServerManager.Instance.DictRefToNetData)
            {
                if (runner.LocalPlayer == data.Key)
                {
                    ServerManager.Instance.IsAllReadyAction = SetActiveButton;
                    continue; 
                }
                data.Value.IsReady = false;
            }
            bool isAllReday = ServerManager.Instance.CheckAllPlayerIsReadyInServer();
            ServerManager.Instance.IsAllReadyAction(isAllReday);
        }
       // gameObject.SetActive(true);
    }

    private void ClickReadyButton()
    {
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer) return;
        IsReady = !IsReady;
        ImgBtnColor.color = IsReady ? OnReadyColor : OffReadyColor;
        ServerManager.Instance.ThisPlayerData.Rpc_PlayerIsReady(IsReady);
    }

    private void ClickStartButton()
    {
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (!(runner.IsServer)) return;
        ServerManager.Instance.ThisPlayerData.Rpc_MoveScene(ESceneName.BattleScene);
    }

    private void SetActiveButton(bool isAllReady)
    {
        BtnReadyOrStart.interactable = isAllReady;
    }
}
