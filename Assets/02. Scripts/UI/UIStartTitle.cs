using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStartTitle : UIPermanent
{
    [field: SerializeField] public TextMeshProUGUI TextName { get; private set;}
    [field: SerializeField] public TextMeshProUGUI TextSoloClearTime { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TextMultiClearTime { get; private set; }
    private void Awake()
    {
        ServerManager.Instance.UIStartTitle = this;
        SoloClearTimeUpdate();
        MultiClearTimeUpdate();
    }

    public void TextUpdate()
    {
        TextName.text = ServerManager.Instance.PlayerName;
    }

    public void SoloClearTimeUpdate()
    {
        TextSoloClearTime.text = GameValueManager.Instance.GameSoloClearTime;
    }

    public void MultiClearTimeUpdate()
    {
        TextMultiClearTime.text = GameValueManager.Instance.GameMultiClearTime;
    }
}
