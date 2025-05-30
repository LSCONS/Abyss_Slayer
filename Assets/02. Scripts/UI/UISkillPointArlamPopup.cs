using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillPointArlamPopup : UIPopup
{
    [field: SerializeField] public Button BtnYesClick { get; private set; }
    [field: SerializeField] public Button BtnNoClick { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextDescription { get; private set; }
    public string strServerText = "아직 포인트가 남아 있습니다.\n\n그래도 시작하시겠습니까?";
    public string strClientText = "아직 포인트가 남아 있습니다.\n\n그래도 준비하시겠습니까?";

    public UIReadyBossStage UIReadyBossStage { get; set; }

    public void Awake()
    {
        BtnYesClick.onClick.AddListener(ClickYesButton);
        BtnNoClick.onClick.AddListener(ClickNoButton);
    }

    private void ClickYesButton()
    {
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        NetworkRunner runner = RunnerManager.Instance.GetRunner();
        if (runner.IsServer)
        {
            UIReadyBossStage.ServerActionButton();
        }
        else
        {
            UIReadyBossStage.ClientActionButton();
        }
        OnClose();
    }


    private void ClickNoButton()
    {
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        OnClose();
    }
}
