using Fusion;
using Photon.Realtime;
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
    public string ServerText { get; private set; } = "게임 시작";
    public string ClientText { get; private set; } = "게임 준비";
    private bool IsReady { get; set; } = false;
    [field: SerializeField] public Color OnReadyColor { get; private set; }
    [field: SerializeField] public Color OffReadyColor { get; private set; }
    [field: SerializeField] public UISkillPointArlamPopup  UISkillPointArlamPopup { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        ServerManager.Instance.UIReadyBossStage = this;
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
        BtnReadyOrStart.onClick.RemoveListener(PlayClickSound);
       BtnReadyOrStart.onClick.AddListener(PlayClickSound);
        UISkillPointArlamPopup.UIReadyBossStage = this;
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
    }

    private void ClickReadyButton()
    {
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer) return;
        Player player = ServerManager.Instance.ThisPlayer;
        //준비가 안 된 상태에서 스킬 포인트가 남아 있을 경우
        if(!(IsReady) && player.SkillPoint.Value + player.StatPoint.Value > 0)
        {
            UISkillPointArlamPopup.TextDescription.text = UISkillPointArlamPopup.strClientText;
            UISkillPointArlamPopup.OnOpen();
        }
        else
        {
            ClientActionButton();
        }
    }

    private void ClickStartButton()
    {
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (!(runner.IsServer)) return;
        Player player = ServerManager.Instance.ThisPlayer;
        if (player.SkillPoint.Value + player.StatPoint.Value > 0)
        {
            UISkillPointArlamPopup.TextDescription.text = UISkillPointArlamPopup.strServerText;
            UISkillPointArlamPopup.OnOpen();
        }
        else
        {
            ServerActionButton();
        }
    }

    public void ResetClientButton()
    {
        if (RunnerManager.Instance.GetRunner().IsServer) return;
        IsReady = false;
        ImgBtnColor.color = OffReadyColor;
        ServerManager.Instance.ThisPlayerData.Rpc_PlayerIsReady(IsReady);
    }

    public void ClientActionButton()
    {
        IsReady = !IsReady;
        ImgBtnColor.color = IsReady ? OnReadyColor : OffReadyColor;
        ServerManager.Instance.ThisPlayerData.Rpc_PlayerIsReady(IsReady);
    }

    public void ServerActionButton()
    {
        ServerManager.Instance.ThisPlayerData.Rpc_MoveScene(ESceneName.BattleScene);
    }

    public void SetActiveButton(bool isAllReady)
    {
        BtnReadyOrStart.interactable = isAllReady;
    }
}
