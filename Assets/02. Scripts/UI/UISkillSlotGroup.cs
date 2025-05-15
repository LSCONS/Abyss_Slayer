using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillSlotGroup : UIPermanent
{
    public override void Init()
    {
        base.Init();
        gameObject.SetActive(true);
        UISkillSlotManager.Instance.Init();
        UIBuffSlotManager.Instance.Init();
        RectTransform rect = transform.parent.GetComponent<RectTransform>();
        if (rect != null) LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
}
