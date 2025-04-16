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

    // 스킬 툴팁 위치 설정
    public void SetTooltipPosition(Vector3 position)
    {
        var tooltip = UIManager.Instance.GetUI<UISkillTooltip>();

        // position의 위치에 더해서 툴팁 표시
        tooltip.transform.position = position + new Vector3(150f, 150f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        tooltip.Open();

        // 툴팁이 열려있지 않으면 열기
        if (!tooltip.gameObject.activeSelf)
            tooltip.Open();

        tooltip.SetSkill(skillData);

        // 마우스 위치를 기준으로 툴팁 설정
        SetTooltipPosition(eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        tooltip.Close();
    }

}
