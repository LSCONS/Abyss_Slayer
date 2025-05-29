using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBossState : UIPermanent
{
    [field: SerializeField] public UIHealthBar UIHealthBar { get; set; }

    private void Awake()
    {
        ServerManager.Instance.UIBossState = this;
    }
}
