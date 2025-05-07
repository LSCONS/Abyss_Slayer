using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPopup : UIBase
{
    private CanvasGroup canvasGroup;

    private Tween popTween;
    private Tween openTween;
    private Tween closeTween;

    public void OnDisable()
    {
        if (UIManager.Instance != null && UIManager.Instance.popupStack.Contains(this))
        {
            UIManager.Instance.CloseCurrentPopup(this);
        }
    }

    public override void Init()
    {
        base.Init();
        // 팝업은 시작할 때 닫아두기
        this.gameObject.SetActive(false);
        if(canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if(canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    public override void Open(params object[] args)
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        base.Open(args);
        transform.SetAsLastSibling();   // 제일 위로 올려줌


        // 애니메이션 추가
        // 시작 상태
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        transform.localScale = Vector3.one * 0.8f;

        popTween?.Kill();
        popTween = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack))
            .Join(canvasGroup.DOFade(1, 0.25f))
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public override void Close()
    {

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        popTween?.Kill();
        popTween = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one * 0.8f, 0.2f).SetEase(Ease.InBack))
            .Join(canvasGroup.DOFade(0, 0.2f))
            .OnComplete(() =>
            {
                base.Close();
            });


    }

    /// <summary>
    /// 열기 / 버튼 이거 연결해주면 됨
    /// </summary>
    public virtual void OnOpen()
    {
        Open();
    }

    /// <summary>
    /// 닫기 / 버튼 이거 연결해주면 됨
    /// </summary>
    public virtual void OnClose()
    {
        Close();
        UIManager.Instance.CloseCurrentPopup(this);
    }


}
