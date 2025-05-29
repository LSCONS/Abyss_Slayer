using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillPointArlamPopup : UIPopup
{
    [field: SerializeField] public Button BtnYesClick { get; private set; }
    [field: SerializeField] public Button BtnNoClick { get; private set; }
    public UIReadyBossStage UIReadyBossStage { get; set; }

    public void Awake()
    {
        BtnYesClick.onClick.AddListener(ClickYesButton);
        BtnNoClick.onClick.AddListener(ClickNoButton);
    }

    private void ClickYesButton()
    {
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
        OnClose();
    }
}
