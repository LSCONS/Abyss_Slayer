using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
public class UIBuffSlot : MonoBehaviour, IView
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image coolTimeOverlay;
    [SerializeField] private TextMeshProUGUI coolTimeText;
    private IPresenter presenter;

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    public void SetCoolTime(float coolTime)
    {
        coolTimeOverlay.fillAmount = coolTime;
        coolTimeText.text = coolTime.ToString("F0");
    }

    public void SetPresenter(IPresenter presenter)
    {
        this.presenter = presenter;
    }

}
