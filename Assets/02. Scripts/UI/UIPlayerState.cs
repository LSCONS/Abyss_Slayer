using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerState : UIPermanent
{
    [field: SerializeField] public UIHealthBar UIHealthBar { get; set; }

    private void Awake()
    {
        ServerManager.Instance.UIPlayerState = this;
    }
}
