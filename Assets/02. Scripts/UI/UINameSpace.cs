using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UINameSpace : UIPermanent
{
    [field: SerializeField] public TextMeshProUGUI TextName { get; private set;}

    private void Awake()
    {
        ServerManager.Instance.ChangeNameAction += () => TextName.text = ServerManager.Instance.PlayerName;
    }
}
