using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISkillUpgradeController : UIPopup
{
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] private Transform upgradeSlotParent;

    [SerializeField] private TextMeshProUGUI skillPointText;
    [ReadOnlyAttribute] private int skillPoint = 0;



    private Dictionary<Skill, UISkillSlot> upgradeSlots = new();

    public override void Init()
    {
        base.Init();

        skillPoint = PlayerManager.Instance.Player.SkillPoint;
        UpdateSkillPointText();

        foreach (var skill in PlayerManager.Instance.Player.EquippedSkills.Values)
        {
            var go = Instantiate(skillSlotPrefab, upgradeSlotParent);
            var slot = go.GetComponent<UISkillSlot>();

            // 슬롯 세팅
            slot.SetSkillData(skill);
            slot.SetIcon(skill.SkillIcon);
            slot.SetName(skill);
            slot.SetSkillLevel(skill.Level.Value);

            // 버튼 세팅
            var buttons = slot.GetComponentsInChildren<Button>();
            var upgradeButton = buttons[0];
            var downgradeButton = buttons[1];


            upgradeButton.onClick.AddListener(() =>
            {
                if (skillPoint > 0)
                {
                    skill.SkillUpgrade();
                    skillPoint--;
                    slot.SetSkillLevel(skill.Level.Value);
                    UpdateSkillPointText();
                }
            });

            downgradeButton.onClick.AddListener(() =>
            {
                if (skill.Level.Value > 1)
                {
                    skill.Level.Value--;
                    skillPoint++;
                    slot.SetSkillLevel(skill.Level.Value);
                    UpdateSkillPointText();
                }
            });

            upgradeSlots.Add(skill, slot);
        }
    }

    private void UpdateSkillPointText()
    {
        skillPointText.text = $"스킬 포인트: {skillPoint}";
    }

}
    
