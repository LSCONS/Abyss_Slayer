using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIPopup : UIBase
{
    private CanvasGroup canvasGroup;

    private Tween popTween;
    private Tween openTween;
    private Tween closeTween;

    [field: Header("설명을 바꾸고싶다면 설정해주세요.")]
    [field: SerializeField] public TextMeshProUGUI desc { get;private set; }
    private string descriptionText;

    public virtual void OnDisable()
    {
        if (ManagerHub.Instance.UIManager != null && ManagerHub.Instance.UIManager.PopupStack.Contains(this))
        {
            ManagerHub.Instance.UIManager.CloseCurrentPopup(this);
        }
    }

    public override void Init()
    {
        base.Init();
        if(!string.IsNullOrWhiteSpace(descriptionText)) SetDesc(descriptionText);
        // 팝업은 시작할 때 닫아두기
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
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 팝업 열기
    /// </summary>
    public override void Open(params object[] args)
    {
        popTween?.Kill();

        base.Open(args);
        if (desc != null && !string.IsNullOrWhiteSpace(descriptionText))
        {
            desc.text = descriptionText;
        }


        transform.SetAsLastSibling();   // 제일 위로 올려줌


        // 애니메이션 추가
        // 시작 상태
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        transform.localScale = Vector3.one * 0.8f;

        popTween = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack))
            .Join(canvasGroup.DOFade(1, 0.25f))
            .OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                popTween = null;
            });
    }

    /// <summary>
    /// 팝업 닫기
    /// </summary>
    public override void Close()
    {
        // ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);

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
        ManagerHub.Instance.UIManager.CloseCurrentPopup(this);
    }

    public void SetDesc(string text)
    {
        descriptionText = text;
    }
}
