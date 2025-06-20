using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Threading.Tasks;
using Fusion;
public class UIHealthBar : UIPermanent, IView
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image shadowBar; // hp바 그림자 (체력 깎일 때 사용)
    [SerializeField] private IPresenter presenter;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private RectTransform shakeTarget; // 체력 닳을 때 흔들릴 타겟

    public void ConnectPlayerObject(Player player)
    {
        bool bindSuccess = UIBinder.BindPlayer<IHasHealth, UIHealthBar, HealthPresenter>(player, this.gameObject);
        if (!bindSuccess)
        {
            Debug.LogWarning($"[UIHealthBar] 플레이어 바인딩 실패: {player}");
        }
        hpBar.fillAmount = 1;
        hpText.text = $"{100:F0}%";
    }

    public void ConnectOtherPlayerObject(PlayerRef playerRef)
    {
        bool bindSuccess = UIBinder.BindPlayer<IHasHealth, UIHealthBar, HealthPresenter>(ManagerHub.Instance.ServerManager.DictRefToPlayer[playerRef], this.gameObject);
        if (!bindSuccess)
        {
            Debug.LogWarning($"[UIHealthBar] 플레이어 바인딩 실패: {ManagerHub.Instance.ServerManager.ThisPlayer}");
        }
        hpBar.fillAmount = 1;
        hpText.text = $"{100:F0}%";
    }

    public void ConnectBossObject(Boss boss)
    {
        bool bindSuccess = UIBinder.BindBoss<IHasHealth, UIHealthBar, HealthPresenter>(boss, this.gameObject);
        if (!bindSuccess)
        {
            Debug.LogWarning($"[UIHealthBar] 보스 바인딩 실패: {ManagerHub.Instance.ServerManager.ThisPlayer}");
        }
        hpBar.fillAmount = 1;
        hpText.text = $"{100:F0}%";
    }

    private int currentHp = 0;  // 애니메이션에 쓰일 현재 hp
    private Tween hpTween;  // 이전 Tween 저장

    public void SetHp(float ratio, int hp, int maxHp)
    {
        // 체력 애니메이션
        if (shadowBar == null)  // 그림자 없으면 그냥 체력바 애니메이션만
        {
            hpBar.DOFillAmount(ratio, 0.3f).SetEase(Ease.OutQuart);
        }
        else if (shadowBar != null)  // 그림자 있으면 그림자 애니메이션 ( 체력ui는 빠르게 줄고 그림자가 천천히 따라옴)
        {
            hpBar.DOFillAmount(ratio, 0.01f).SetEase(Ease.OutQuart);

            if (shadowBar.fillAmount > ratio)
            {
                shadowBar.fillAmount = Mathf.Max(shadowBar.fillAmount, ratio);
                shadowBar.DOFillAmount(ratio, 1.0f).SetEase(Ease.OutQuart);
            }
            else
            {
                // 체력 회복시는 바로바로
                shadowBar.fillAmount = ratio;
            }
        }

        // 이전 Tween 중지
        hpTween?.Kill();

        // 숫자 애니메이션
        hpTween = DOTween.To(() => currentHp, x =>
        {
            currentHp = x;
            hpText.text = $"{currentHp}/{maxHp}";
        }, hp, 0.3f).SetEase(Ease.OutQuart);

        // 이전보다 hp가 줄었으면 흔들림 효과
        shakeTarget.DOComplete();   // 이전 흔들림 효과 중지
        if (hp < currentHp)
        {
            shakeTarget.DOShakeAnchorPos(0.2f, strength: new Vector2(10f, 0f), vibrato: 10) //  0.2f동안 x축 10f의 강도 흔들림 효과 10번
                .SetEase(Ease.OutCirc);
        }

        // 50이하로 떨어지면 색 변경
        Color targetColor;
        if (ratio > 0.5f)
        {
            targetColor = Color.red;
        }
        else
        {
            float t = Mathf.InverseLerp(0.5f, 0f, ratio);
            Color lowHpStart = new Color(0.4f, 0f, 0f);
            targetColor = Color.Lerp(lowHpStart, Color.black, t);
        }

        hpBar.DOColor(targetColor, 0.3f);
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
