using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UnityEngine.EventSystems;
public class UIBuffSlot : MonoBehaviour, IView, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image coolTimeOverlay;
    [SerializeField] private TextMeshProUGUI coolTimeText;
    private IPresenter presenter;
    private UISkillTooltip tooltip;
    RectTransform slotRect;


    // 이름 설명
    private string buffName;
    private string buffDescription;

    private void Start()
    {
        tooltip = ManagerHub.Instance.UIManager.GetUI<UISkillTooltip>();
        slotRect = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        var tooltip = ManagerHub.Instance.UIManager.GetUI<UISkillTooltip>();
        if (tooltip != null && tooltip.gameObject.activeSelf)
        {
            tooltip.Close();
        }
    }

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

    public void SetBuffInfo(string name, string description)
    {
        buffName = name;
        buffDescription = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var tooltip = ManagerHub.Instance.UIManager.GetUI<UISkillTooltip>();
        tooltip.ShowTooltip(buffName, buffDescription, slotRect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltip = ManagerHub.Instance.UIManager.GetUI<UISkillTooltip>();
        tooltip.Close();
    }

}
