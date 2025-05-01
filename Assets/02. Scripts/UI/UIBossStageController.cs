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

        bossText.text = $"보스{GameFlowManager.Instance.CurrentStageIndex}";

    }

    
}
