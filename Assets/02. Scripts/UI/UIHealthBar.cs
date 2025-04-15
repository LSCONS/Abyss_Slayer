using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UIHealthBar : UIBase, IView
{
    [SerializeField] private Image hpBar;
    [SerializeField] private IPresenter presenter;
    [SerializeField] private TextMeshProUGUI hpText;

    public override void Init()
    {
        hpBar.fillAmount = 1;
        hpText.text = $"{100:F0}%";
    }

    private int currentHp = 0;  // 애니메이션에 쓰일 현재 hp
    private Tween hpTween;  // 이전 Tween 저장
    public void SetHp(float ratio, int hp, int maxHp)
    {
        hpBar.DOFillAmount(ratio, 0.3f).SetEase(Ease.OutQuart);

        // 이전 Tween 중지
        hpTween?.Kill();

        // 숫자 애니메이션
        hpTween = DOTween.To(() => currentHp, x =>
        {
            currentHp = x;
            hpText.text = $"{currentHp}/{maxHp}";
        }, hp, 0.3f).SetEase(Ease.OutQuart);
    }   

    public void SetPresenter(IPresenter presenter)
    {
        this.presenter = presenter;
    }

    private void OnDestroy()
    {
        presenter?.Dispose();
    }


}
