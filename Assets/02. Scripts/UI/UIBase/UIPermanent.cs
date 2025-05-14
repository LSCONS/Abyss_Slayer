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
        //var rect = GetComponent<RectTransform>();
        //var originalPos = rect.anchoredPosition;

        //Vector2 startPos = originalPos;

        //if ((uiType & UIType.Top) != 0)
        //    startPos += new Vector2(0, 0f);
        //else if ((uiType & UIType.Bottom) != 0)
        //    startPos += new Vector2(0, -0f);
        //else if ((uiType & UIType.TopMid) != 0)
        //    startPos += new Vector2(0, 0f);
        //else if ((uiType & UIType.Permanent) != 0)  // 그냥 고정 ui면 애니메이션 없음
        //        return;

        //rect.anchoredPosition = startPos;

        // 등장 애니메이션
        //송제우가 true에서 false로 바꿈.
        //이거 true로 하면 인트로 씬에서도 시작하자마자 보이는 버그가 있음

        //rect.DOAnchorPos(originalPos, 0.5f)
        //    .SetEase(Ease.OutExpo);
    }
}
