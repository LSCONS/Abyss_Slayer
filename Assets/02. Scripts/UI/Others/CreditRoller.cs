using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditRoller : UIPopup
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollDuration = 10;

    private void OnEnable()
    {
        StartCoroutine(ScrollCredits());
    }
    public override void Init()
    {
        base.Init();

    }
    private IEnumerator ScrollCredits()
    {
        // 중앙에서 시작
        var layout = scrollRect.content.GetComponent<VerticalLayoutGroup>();
        layout.padding.top = Mathf.RoundToInt(scrollRect.viewport.rect.height / 2f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

        // 젤 위에서 시작
        scrollRect.verticalNormalizedPosition = 1f;

        float elapsed = 0f;
        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(1f, 0f, elapsed / scrollDuration);
            yield return null;
        }

        scrollRect.verticalNormalizedPosition = 0f;
        // 크레딧 끝났을 때 처리
    }
}
