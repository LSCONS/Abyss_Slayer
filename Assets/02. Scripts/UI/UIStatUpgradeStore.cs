using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Analytics;

public class UIStatUpgradeStore : UIPopup
{
    [SerializeField] TextMeshProUGUI hpLevelText;
    [SerializeField] TextMeshProUGUI damageLevelText;

    [SerializeField] TextMeshProUGUI hpFigure;
    [SerializeField] TextMeshProUGUI damageFigure;

    [SerializeField] TextMeshProUGUI hpTotalFigure;
    [SerializeField] TextMeshProUGUI damageTotalFigure;

    [SerializeField] TextMeshProUGUI remainingPointText;

    [SerializeField] Button hpUpgradeButton;
    [SerializeField] Button hpDowngradeButton;

    [SerializeField] Button damageUpgradeButton;
    [SerializeField] Button damageDowngradeButton;

    [SerializeField] Button applyButton;

    [Header("증가량")]
    [SerializeField] float Amount = 0.1f;


    private int AppliedHpLevel { get; set; } = 0;
    private int TempHpLevel { get; set; } = 0;
    private int AppliedDamageLevel { get; set; } = 0;
    private int TempDamageLevel { get; set; } = 0;
    private int RemainingPoint { get; set; } = 0;
    private int OriginalPoint { get; set; } = 0;    // 원본 저장

    private int BaseMaxHp { get; set; } = 0;
    private float BaseDamage { get; set; } = 0;

    public void Awake()
    {
#if AllMethodDebug
        Debug.Log("Awake");
#endif
        ManagerHub.Instance.UIConnectManager.UIStatUpgradeStore = this;
    }
    public override async void Init()
    {
# if AllMethodDebug
        Debug.Log("Init");
#endif
        base.Init();
        Player player = await ManagerHub.Instance.ServerManager.WaitForThisPlayerAsync();

        OriginalPoint = player.StatPoint.Value;       // 저장
        RemainingPoint = OriginalPoint;

        BaseMaxHp = player.BaseMaxHp;
        BaseDamage = player.BaseDamage;

        // 처음 시작할 때 이전 레벨 저장해야됨
        AppliedHpLevel = player.HpStatLevel;
        AppliedDamageLevel = player.DamageStatLevel;

        TempHpLevel = player.HpStatLevel;
        TempDamageLevel = player.DamageStatLevel;

        // 버튼 해제
        OffConnectButton();
        // 버튼 저장
        OnConnectButton();
        // 업데이트 UI
        UpdateUI();
        applyButton.interactable = false;
        SetAllDowngradeBtn(false);
        if (RemainingPoint > 0) SetAllUpgradeBtn(true);
    }
    
    public override void OnDisable()
    {
# if AllMethodDebug
        Debug.Log("OnDisable");
#endif
        base.OnDisable();
    }

    public override void OnOpen()
    {
#if AllMethodDebug
        Debug.Log("OnOpen");
#endif
        if (ManagerHub.Instance.ServerManager.ThisPlayer.StatPoint.Value > 0) SetAllUpgradeBtn(true);
        base.OnOpen();
    }

    public override void Close()
    {
# if AllMethodDebug
        Debug.Log("Close");
#endif
        base.Close();
        ResetUnappliedChange();

    }

    private void OnConnectButton()
    {
# if AllMethodDebug
        Debug.Log("OnConnectButton");
#endif
        hpUpgradeButton.onClick.AddListener(OnHpUpgrade);
        hpDowngradeButton.onClick.AddListener(OnHpDowngrade);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgrade);
        damageDowngradeButton.onClick.AddListener(OnDamageDowngrade);
        applyButton.onClick.AddListener(ApplyStatsToPlayer);
    }
    private void OffConnectButton()
    {
# if AllMethodDebug
        Debug.Log("OffConnectButton");
#endif
        hpUpgradeButton.onClick.RemoveListener(OnHpUpgrade);
        hpDowngradeButton.onClick.RemoveListener(OnHpDowngrade);
        damageUpgradeButton.onClick.RemoveListener(OnDamageUpgrade);
        damageDowngradeButton.onClick.RemoveListener(OnDamageDowngrade);
        applyButton.onClick.RemoveListener(ApplyStatsToPlayer);
    }

    private void UpdateUI()
    {
# if AllMethodDebug
        Debug.Log("UpdateUI");
#endif
        var player = ManagerHub.Instance.ServerManager.ThisPlayer;

        hpLevelText.text = $"lv.{TempHpLevel}";
        damageLevelText.text = $"lv.{TempDamageLevel}";

        int appliedHpLevel = player.HpStatLevel;
        int appliedDmgLevel = player.DamageStatLevel;

        int hpIncrease = Mathf.RoundToInt(BaseMaxHp * Amount * (TempHpLevel - appliedHpLevel));
        hpFigure.text = $"+{hpIncrease}";

        float damageIncrease = BaseDamage * Amount * (TempDamageLevel - appliedDmgLevel);
        damageFigure.text = $"+{damageIncrease * 100:f0}%";

        int totalHpIncrease = Mathf.RoundToInt(BaseMaxHp * Amount * (TempHpLevel - 1));
        hpTotalFigure.text = $"+{totalHpIncrease}";

        float totalDamageIncrease = BaseDamage * Amount * (TempDamageLevel - 1);
        damageTotalFigure.text = $"+{totalDamageIncrease * 100:f0}%";

        remainingPointText.text = $"남은 포인트: {RemainingPoint}";
    }

    void OnHpUpgrade()
    {
# if AllMethodDebug
        Debug.Log("OnHpUpgrade");
#endif
        if (RemainingPoint > 0)
        {
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
            TempHpLevel++;
            RemainingPoint--;
            UpdateUI();
            applyButton.interactable = true;
            hpDowngradeButton.interactable = true;
            if (RemainingPoint == 0) SetAllUpgradeBtn(false);
        }
    }

    void OnHpDowngrade()
    {
# if AllMethodDebug
        Debug.Log("OnHpDowngrade");
#endif
        if (TempHpLevel > AppliedHpLevel)
        {
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
            TempHpLevel--;
            RemainingPoint++;
            UpdateUI();
            if(TempHpLevel == AppliedHpLevel)hpDowngradeButton.interactable = false;
            if (RemainingPoint == OriginalPoint) applyButton.interactable = false;
            SetAllUpgradeBtn(true);
        }
    }

    void OnDamageUpgrade()
    {
# if AllMethodDebug
        Debug.Log("OnDamageUpgrade");
#endif
        if (RemainingPoint > 0)
        {
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
            TempDamageLevel++;
            RemainingPoint--;
            UpdateUI();
            applyButton.interactable = true;
            damageDowngradeButton.interactable = true;
            if (RemainingPoint == 0) SetAllUpgradeBtn(false);
        }
    }

    void OnDamageDowngrade()
    {
# if AllMethodDebug
        Debug.Log("OnDamageDowngrade");
#endif
        if (TempDamageLevel > AppliedDamageLevel)
        {
            ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
            TempDamageLevel--;
            RemainingPoint++;
            UpdateUI();
            if(TempDamageLevel == AppliedDamageLevel)damageDowngradeButton.interactable = false;
            if(RemainingPoint == OriginalPoint)applyButton.interactable = false;
            SetAllUpgradeBtn(true);
        }
    }

    private void ApplyStatsToPlayer()
    {
# if AllMethodDebug
        Debug.Log("ApplyStatsToPlayer");
#endif
        ManagerHub.Instance.SoundManager.PlaySFX(EAudioClip.SFX_ButtonClick);
        var player = ManagerHub.Instance.ServerManager.ThisPlayer;

        player.Rpc_ApplyStatUpgrade(TempHpLevel, TempDamageLevel, Amount);

        // 스텟 포인트 반영
        OriginalPoint = RemainingPoint;
        AppliedHpLevel = TempHpLevel;
        AppliedDamageLevel = TempDamageLevel;
        player.StatPoint.Value = RemainingPoint;

        // 스탯 업그레이드 애널리틱스 전송
        string stageNumber = ManagerHub.Instance.ServerManager.BossCount.ToString();
        string classType = player.NetworkData.Class.ToString();
        if (TempHpLevel > AppliedHpLevel)
        {
            UpgradeAnalytics.SendClassStatUpgradeInfo(stageNumber, classType, "HP", TempHpLevel);
        }
        if (TempDamageLevel > AppliedDamageLevel)
        {
            UpgradeAnalytics.SendClassStatUpgradeInfo(stageNumber, classType, "Damage", TempDamageLevel);
        }

        UpdateUI();
        applyButton.interactable = false;
        SetAllDowngradeBtn(false);
        if (RemainingPoint > 0) SetAllUpgradeBtn(true);
    }

    private void ResetUnappliedChange()
    {
# if AllMethodDebug
        Debug.Log("ResetUnappliedChange");
#endif
        RemainingPoint = OriginalPoint;

        TempHpLevel = AppliedHpLevel;
        TempDamageLevel = AppliedDamageLevel;
        UpdateUI();
        applyButton.interactable = false;
        SetAllDowngradeBtn(false);
        if(RemainingPoint > 0)SetAllUpgradeBtn(true);
    }


    private void SetAllUpgradeBtn(bool isInteraction)
    {
# if AllMethodDebug
        Debug.Log("SetAllUpgradeBtn");
#endif
        damageUpgradeButton.interactable = isInteraction;
        hpUpgradeButton.interactable = isInteraction;
    }

    private void SetAllDowngradeBtn(bool isInteraction)
    {
# if AllMethodDebug
        Debug.Log("SetAllDowngradeBtn");
#endif
        damageDowngradeButton.interactable = isInteraction;
        hpDowngradeButton.interactable = isInteraction;
    }
}
