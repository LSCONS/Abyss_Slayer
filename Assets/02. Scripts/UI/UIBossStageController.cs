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

        var state = GameFlowManager.Instance.CurrentState;

        if (state is RestState)
        {
            bossText.text = "휴게실";
        }
        else if (state is BattleState inGame)
            bossText.text = $"보스{GameValueManager.Instance.CurrentStageIndex}";
        else
            bossText.text = string.Empty;

    }

    
}
