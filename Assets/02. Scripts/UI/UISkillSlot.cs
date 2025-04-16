using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UnityEngine.EventSystems;
public class UISkillSlot : MonoBehaviour, IView, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image coolTimeOverlay;
    [SerializeField] private TextMeshProUGUI coolTimeText;
    private IPresenter presenter;

    private SkillData skillData;

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

    public void SetSkillData(SkillData skillData)
    {
        this.skillData = skillData;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        tooltip.Open();
        tooltip.SetSkill(skillData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        tooltip.Close();
    }

}
