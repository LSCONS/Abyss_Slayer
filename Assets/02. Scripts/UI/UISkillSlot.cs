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
    [SerializeField] private TextMeshProUGUI skillName;

    [SerializeField] private Image coolTimeOverlay;
    [SerializeField] private TextMeshProUGUI coolTimeText;
    [SerializeField] private TextMeshProUGUI skillLevelText;

    RectTransform slotRect;
    private UISkillTooltip tooltip;

    private void Start()
    {
        tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        slotRect = GetComponent<RectTransform>();
    }
    private void OnDisable()
    {
        var tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        if (tooltip != null && tooltip.gameObject.activeSelf)
        {
            tooltip.Close();
        }
    }


    private IPresenter presenter;

    private Skill skillData;

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    public void SetName(Skill skillData)
    {
        skillName.text = skillData.SkillName;
    }

    public void SetCoolTime(float curCoolTime, float maxCoolTime)
    {
        coolTimeOverlay.fillAmount = curCoolTime / maxCoolTime;
        if(curCoolTime == 0) coolTimeText.text = "";
        else coolTimeText.text = (curCoolTime).ToString("F1");
    }

    public void SetPresenter(IPresenter presenter)
    {
        this.presenter = presenter;
    }

    public void SetSkillData(Skill skillData)
    {
        this.skillData = skillData;
    }

    public void SetSkillLevel(int level)
    {
        skillLevelText.text = $"Lv.{level}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.ShowTooltip(skillData);

        // 마우스 위치를 기준으로 툴팁 설정
        tooltip.SetTooltipPosition(slotRect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        tooltip.Close();
    }

}
