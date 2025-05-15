using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UISkillUpgradeController : UIPopup
{
    [Header("스킬 슬롯 프리팹")]
    [SerializeField] private GameObject skillSlotPrefab;
    [Header("스킬 슬롯 추가될 위치")]
    [SerializeField] private Transform upgradeSlotParent;
    [Header("스킬 포인트 텍스트")]
    [SerializeField] private TextMeshProUGUI skillPointText;
    [Header("적용하기 버튼")]
    [SerializeField] private Button applyButton;

    private int skillPoint = 0;
    private int tempLevel = 0;
    private Dictionary<Skill, UISkillSlot> upgradeSlots = new();

    private class SkillUpgradeData
    {
        public Skill skill;
        public int TempLevel;
    }
    private Dictionary<Skill, SkillUpgradeData> upgradeData = new();

    public override void Init()
    {
        base.Init();
        // 스킬 포인트 초기화
        skillPoint = PlayerManager.Instance.Player.SkillPoint;
        UpdateSkillPointText();

        // 슬롯이 이미 생성되어 있으면 초기화 생략해야됨
        if (upgradeSlots.Count > 0) return;


        // 슬롯 설정
        foreach (var skill in PlayerManager.Instance.Player.EquippedSkills.Values)
        {
            var go = Instantiate(skillSlotPrefab, upgradeSlotParent);
            var slot = go.GetComponent<UISkillSlot>();

            SetSlots(slot, skill);      // 슬롯 세팅
            SetButtons(slot, skill);    // 버튼 연결

            // 업그레이드 데이터 설정
            upgradeData[skill] = new SkillUpgradeData
            {
                skill = skill,
                TempLevel = tempLevel
            };
        }

        // 버튼 초기화
        applyButton.onClick.RemoveAllListeners();
        applyButton.onClick.AddListener(ApplayUpgrade);
    }

    /// <summary>
    /// 슬롯 세팅
    /// </summary>
    private void SetSlots(UISkillSlot slot, Skill skill)
    {
        slot.SetSkillData(skill);
        slot.SetIcon(skill.SkillIcon);
        slot.SetName(skill);
        slot.SetSkillLevel(skill.Level.Value);
    }

    /// <summary>
    /// 버튼 세팅
    /// </summary>
    private void SetButtons(UISkillSlot slot, Skill skill)
    {
        // 버튼 세팅
        var buttons = slot.GetComponentsInChildren<Button>();
        var upgradeButton = buttons[0];     // 첫번째 버튼이 업그레이드
        var downgradeButton = buttons[1];   // 두번째 버튼이 다운그레이드임

        // 업그레이드 버튼 누르면 템프 레벨 올리고 스포 내리고
        upgradeButton.onClick.AddListener(() =>
        {
            var data = upgradeData[skill];
            if (skillPoint > 0)
            {
                data.TempLevel++;                         // 임시 레벨 올림
                skillPoint--;                             // 스킬 포인트 낮춤
                slot.SetSkillLevel(data.TempLevel);       // 슬롯의 레벨 텍스트 수정
                UpdateSkillPointText();                   // 상점의 스킬 포인트 수정
            }                                            
        });                                              
                                                         
        downgradeButton.onClick.AddListener(() =>        
        {                                                
            var data = upgradeData[skill];               
                                                         
            if (data.TempLevel > skill.Level.Value)      
            {                                            
                data.TempLevel--;                         // 임시 레벨 올림
                skillPoint++;                             // 스킬 포인트 낮춤
                slot.SetSkillLevel(data.TempLevel);       // 슬롯의 레벨 텍스트 수정
                UpdateSkillPointText();                   // 상점의 스킬 포인트 수정
            }
        });

        upgradeSlots.Add(skill, slot);
    }

    private void UpdateSkillPointText()
    {
        skillPointText.text = $"스킬 포인트: {skillPoint}";
    }

    private void ApplayUpgrade()
    {
        foreach ( var upData in upgradeData )
        {
            var skill = upData.Key;
            var data = upData.Value;

            int levelDiff = data.TempLevel - skill.Level.Value;

            for(int i = 0; i < levelDiff; i++)
            {
                skill.SkillUpgrade();
            }
        }

        // 스킬 포인트 반영해줌
        PlayerManager.Instance.Player.SkillPoint = skillPoint;
    }
}
    
