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
        bossText = GetComponent<TextMeshProUGUI>();
        bossText.text = $"보스_{GameFlowManager.Instance.currentState}";

    }

    
}
