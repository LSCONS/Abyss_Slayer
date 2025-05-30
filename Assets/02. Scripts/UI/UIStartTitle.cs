using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStartTitle : UIPermanent
{
    [field: SerializeField] public TextMeshProUGUI TextName { get; private set;}
    [field: SerializeField] public TextMeshProUGUI TextClearTime { get; private set;}
    private void Awake()
    {
        ServerManager.Instance.UIStartTitle = this;
        ClearTimeUpdate();
    }

    public void TextUpdate()
    {
        TextName.text = ServerManager.Instance.PlayerName;
    }

    public void ClearTimeUpdate()
    {
        TextClearTime.text = GameValueManager.Instance.GameClearTime;
    }
}
