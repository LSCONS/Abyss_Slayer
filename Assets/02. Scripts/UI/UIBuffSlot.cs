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

    public void SetCoolTime(float curCoolTime, float maxCoolTime)
    {
        coolTimeOverlay.fillAmount = curCoolTime / maxCoolTime;
        if (curCoolTime == 0) coolTimeText.text = "";
        else coolTimeText.text = (curCoolTime).ToString("F0");
    }

    public void SetPresenter(IPresenter presenter)
    {
        this.presenter = presenter;
    }

}
