using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPermanent : UIBase
{
    public override void Init()
    {
        base.Init();
        // 시작 위치 설정
        var rect = GetComponent<RectTransform>();
        var originalPos = rect.anchoredPosition;

        Vector2 startPos = originalPos;

        if ((uiType & UIType.Top) != 0)
            startPos += new Vector2(0, 200f);
        else if ((uiType & UIType.Bottom) != 0)
            startPos += new Vector2(0, -200f);
        else if ((uiType & UIType.TopMid) != 0)
            startPos += new Vector2(0, -200f);
        else if ((uiType & UIType.Permanent) != 0)  // 그냥 고정 ui면 애니메이션 없음
                return;

        rect.anchoredPosition = startPos;

        // 등장 애니메이션
        gameObject.SetActive(true); // 애니메이션 보기 위해 켜야 함

        rect.DOAnchorPos(originalPos, 0.5f)
            .SetEase(Ease.OutExpo);
    }
}
