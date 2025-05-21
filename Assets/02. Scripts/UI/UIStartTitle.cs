using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStartTitle : UIPermanent
{
    [field: SerializeField] public TextMeshProUGUI TextName { get; private set;}
    private void Awake()
    {
        ServerManager.Instance.UIStartTitle = this;
    }

    public void TextUpdate()
    {
        TextName.text = ServerManager.Instance.PlayerName;
    }
}
