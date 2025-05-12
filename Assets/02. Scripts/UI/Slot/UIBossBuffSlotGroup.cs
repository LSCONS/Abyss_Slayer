using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBossBuffSlotGroup : UIPermanent
{
    public override void Init()
    {
        base.Init();
        this.gameObject.SetActive(true);
        // 보스 ui 정리
        UIBossBuffSlotManager.Instance.ClearAllSlots();
    }
}
