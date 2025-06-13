using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBossStageController : UIBase
{
    [SerializeField] private TextMeshProUGUI bossText;
    public override void Init()
    {
        base.Init();

        var state = ManagerHub.Instance.GameFlowManager.CurrentState;

        if (state is RestState)
        {
            bossText.text = "휴게실";
        }
        else if (state is BattleState inGame)
            bossText.text = $"보스{ManagerHub.Instance.GameValueManager.CurrentStageIndex}";
        else
            bossText.text = string.Empty;

    }

    
}
