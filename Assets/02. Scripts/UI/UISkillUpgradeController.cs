using Analytics;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
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


    private int SkillPoint { get; set; } = 0;
    private int OriginalSkillPoint { get; set; } = 0;    // 원래 스킬 포인트
    private Dictionary<Skill, UISkillSlot> UpgradeSlots { get; set; } = new();

    private class SkillUpgradeData
    {
        public Skill skill;
        public int TempLevel;
    }
    private Dictionary<Skill, SkillUpgradeData> upgradeData = new();

    public async override void Init()
    {
        base.Init();
        // 스킬 포인트 초기화
        Player player = await ServerManager.Instance.WaitForThisPlayerAsync();
        OriginalSkillPoint = player.SkillPoint.Value;      // 저장
        SkillPoint = OriginalSkillPoint;
        UpdateSkillPointText();

        // 슬롯이 이미 생성되어 있으면 초기화 생략해야됨
        if (UpgradeSlots.Count > 0) return;


        // 슬롯 설정
        foreach (var skill in player.EquippedSkills.Values)
        {
            if (skill.Level.Value == 0) // 스킬 레벨 0으로 설정된 것들은 스킬 강화 안할거임
                continue;

            var go = Instantiate(skillSlotPrefab, upgradeSlotParent);
            var slot = go.GetComponent<UISkillSlot>();
            slot.Init();
            SetSlots(slot, skill);      // 슬롯 세팅
            SetButtons(slot, skill);    // 버튼 연결

            // 업그레이드 데이터 설정
            upgradeData[skill] = new SkillUpgradeData
            {
                skill = skill,
                TempLevel = skill.Level.Value,  // 스킬 레벨 설정
            };
        }

        // 버튼 초기화
        applyButton.onClick.RemoveAllListeners();
        applyButton.onClick.AddListener(ApplyUpgrade);
        applyButton.interactable = false;
        SetAllDowngradeBtn(false);
        if (ServerManager.Instance.ThisPlayer.SkillPoint.Value > 0) SetAllUpgradeBtn(true);
    }

    public override void OnOpen()
    {
        if (ServerManager.Instance.ThisPlayer.SkillPoint.Value > 0) SetAllUpgradeBtn(true);
        base.OnOpen();
    }

    public override void Close()
    {
        base.Close();
        ResetUnappliedChange(); // 적용 안된 것들 초기화
    }

    public override void OnDisable()
    {
        ResetUnappliedChange();
        base.OnDisable();
    }

    /// <summary>
    /// 슬롯 세팅
    /// </summary>
    private void SetSlots(UISkillSlot slot, Skill skill)
    {
        slot.SetSkillData(skill);
        slot.SetSkillUpgradeText(skill, skill.Level.Value);
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

        if(SkillPoint == 0)
        {
            slot.BtnUpgrade.interactable = false;
            slot.BtnDowngrade.interactable = false;
        }

        // 업그레이드 버튼 누르면 템프 레벨 올리고 스포 내리고
        slot.BtnUpgrade.onClick.AddListener(() =>
        {
            var data = upgradeData[skill];
            if (SkillPoint > 0)
            {
                SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
                data.TempLevel++;                                       // 임시 레벨 올림
                SkillPoint--;                                           // 스킬 포인트 낮춤
                slot.SetSkillLevel(data.TempLevel);                     // 슬롯의 레벨 텍스트 수정
                slot.SetSkillUpgradeText(skill, data.TempLevel);        // 슬롯의 증가량 텍스트 업데이트
                UpdateSkillPointText();                                 // 상점의 스킬 포인트 수정
                slot.BtnDowngrade.interactable = true;                  // 다운그레이드 버튼 활성화
                if (SkillPoint == 0) SetAllUpgradeBtn(false);
                applyButton.interactable = true;
            }
        });

        slot.BtnDowngrade.onClick.AddListener(() =>        
        {                                                
            var data = upgradeData[skill];               
                                                         
            if (data.TempLevel > skill.Level.Value)
            {
                SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
                data.TempLevel--;                                       // 임시 레벨 올림
                SkillPoint++;                                           // 스킬 포인트 낮춤
                slot.SetSkillLevel(data.TempLevel);                     // 슬롯의 레벨 텍스트 수정
                slot.SetSkillUpgradeText(skill, data.TempLevel);        // 슬롯의 증가량 텍스트 업데이트
                UpdateSkillPointText();                                 // 상점의 스킬 포인트 수정
                if(data.TempLevel == skill.Level.Value) slot.BtnDowngrade.interactable = false; //다운그레이드 버튼 비활성화
                SetAllUpgradeBtn(true);
                if (SkillPoint == OriginalSkillPoint) applyButton.interactable = false;
            }
        });
        UpgradeSlots.Add(skill, slot);
    }

    private void UpdateSkillPointText()
    {
        skillPointText.text = $"스킬 포인트: {SkillPoint}";
    }

    private async void ApplyUpgrade()
    {
        SoundManager.Instance.PlaySFX(EAudioClip.SFX_ButtonClick);
        Player player = await ServerManager.Instance.WaitForThisPlayerAsync();
        foreach ( var upData in upgradeData )
        {
            var skill = upData.Key;
            var data = upData.Value;

            int levelDiff = data.TempLevel - skill.Level.Value;

            for(int i = 0; i < levelDiff; i++)
            {
                skill.SkillUpgrade();
                // 스킬 업그레이드 애널리틱스 전송
                string stageNumber = ServerManager.Instance.BossCount.ToString();
                string classType = player.NetworkData.Class.ToString();
                string upgradeSkill = skill.SkillName;
                UpgradeAnalytics.SendClassSkillUpgradeInfo(stageNumber, classType, upgradeSkill, data.TempLevel);
            }
        }
        SetAllDowngradeBtn(false);
        // 스킬 포인트 반영해줌
        player.SkillPoint.Value = SkillPoint;
        applyButton.interactable = false;

        OriginalSkillPoint = SkillPoint;
        UpdateSkillPointText();
    }

    private void SetAllUpgradeBtn(bool isActive)
    {
        foreach(var slot in UpgradeSlots.Values)
        {
            slot.BtnUpgrade.interactable = isActive;
        }
    }

    private void SetAllDowngradeBtn(bool isActive)
    {
        foreach (var slot in UpgradeSlots.Values)
        {
            slot.BtnDowngrade.interactable = isActive;
        }
    }



    // 닫을 때 적용안한 것들 초기화
    private void ResetUnappliedChange()
    {
        // 포인트 돌려놓기
        SkillPoint = OriginalSkillPoint;
        UpdateSkillPointText();

        // 스킬 레벨 돌려놓기
        foreach(var upData in upgradeData)
        {
            var skill = upData.Key;
            var data = upData.Value;

            data.TempLevel = skill.Level.Value;

            if(UpgradeSlots.TryGetValue(skill, out var slot))
            {
                slot.SetSkillLevel(data.TempLevel);
                slot.SetSkillUpgradeText(skill, data.TempLevel);
            }
        }
        if (ServerManager.Instance.ThisPlayer.SkillPoint.Value > 0) SetAllUpgradeBtn(true);
        SetAllDowngradeBtn(false);
        applyButton.interactable = false;
    }
}
    
