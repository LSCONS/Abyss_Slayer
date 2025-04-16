using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillSlotGroup : UIPermanent
{
    public override void Init()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(DelayRebuildLayout());
    }
    private IEnumerator DelayRebuildLayout()
    {
        yield return null; // 한 프레임 대기
        var layout = GetComponent<HorizontalLayoutGroup>();
        if (layout != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
        }
    }
}
