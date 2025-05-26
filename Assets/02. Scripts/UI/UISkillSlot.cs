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

    [SerializeField] private Image iconHold;

    [SerializeField] private Image coolTimeOverlay;
    [SerializeField] private TextMeshProUGUI coolTimeText;
    [SerializeField] private TextMeshProUGUI skillLevelText;

    [SerializeField] private TextMeshProUGUI skillUpgradeText;

    [field: SerializeField] public Button BtnUpgrade;
    [field: SerializeField] public Button BtnDowngrade;

    RectTransform slotRect;
    private UISkillTooltip Tooltip 
    {
        get
        {
            if(tooltip != null) return tooltip;
            return tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        }
    }
    private UISkillTooltip tooltip;

    public void Init()
    {
# if AllMethodDebug
        Debug.Log("Init");
#endif
        tooltip = UIManager.Instance.GetUI<UISkillTooltip>();
        slotRect = GetComponent<RectTransform>();
        if(BtnDowngrade!=null)
            BtnDowngrade.interactable = false;
    }
    private void OnDisable()
    {
        Tooltip.Close();
    }

    private IPresenter presenter;

    private Skill skillData;

    public void SetIcon(Sprite icon)
    {
# if AllMethodDebug
        Debug.Log("SetIcon");
#endif
        iconImage.sprite = icon;
    }

    public void SetName(Skill skillData)
    {
# if AllMethodDebug
        Debug.Log("SetName");
#endif
        skillName.text = skillData.SkillName;
    }

    public void SetHoldIcon(bool isshow)
    {
        iconHold?.gameObject.SetActive(isshow);
    }


    public void SetCoolTime(float curCoolTime, float maxCoolTime)
    {
# if AllMethodDebug && Update
        Debug.Log("SetCoolTime");
#endif
        coolTimeOverlay.fillAmount = curCoolTime / maxCoolTime;
        if(curCoolTime == 0) coolTimeText.text = "";
        else coolTimeText.text = (curCoolTime).ToString("F1");
    }

    public void SetPresenter(IPresenter presenter)
    {
# if AllMethodDebug
        Debug.Log("SetPresenter");
#endif
        this.presenter = presenter;
    }

    public void SetSkillData(Skill skillData)
    {
# if AllMethodDebug
        Debug.Log("SetSkillData");
#endif
        this.skillData = skillData;
    }

    public void SetSkillLevel(int level)
    {
# if AllMethodDebug
        Debug.Log("SetSkillLevel");
#endif
        skillLevelText.text = $"Lv.{level}";
    }

    public void SetSkillUpgradeText(Skill skill, int level)
    {
# if AllMethodDebug
        Debug.Log("SetSkillUpgradeText");
#endif
        float percent = skill.Magnification * 100f * (level - 1);
        string typeText = skill.isDurationUp ? "s" : "dmg";
        skillUpgradeText.text = $"+{percent}% ({typeText})";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
# if AllMethodDebug
        Debug.Log("OnPointerEnter");
#endif
        //slotRect = GetComponent<RectTransform>();
        Tooltip.ShowTooltip(skillData, slotRect);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
# if AllMethodDebug
        Debug.Log("OnPointerExit");
#endif
        Tooltip.Close();
    }

}
