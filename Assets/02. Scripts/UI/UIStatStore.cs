using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIStatStore : UIPopup
{
    [SerializeField] TextMeshProUGUI hpLevelText;
    [SerializeField] TextMeshProUGUI damageLevelText;

    [SerializeField] TextMeshProUGUI hpFigure;
    [SerializeField] TextMeshProUGUI damageFigure;

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
    private int OriginalStatPoint { get; set; } = 0;    // 원본 저장

    private int BaseMaxHp { get; set; } = 0;
    private float BaseDamage { get; set; } = 0;

    public override async void Init()
    {
        base.Init();
        Player player = await ServerManager.Instance.WaitForThisPlayerAsync();
        remainingPointValue = player.StatPoint;

        OriginalStatPoint = player.StatPoint;       // 저장
        RemainingPoint = OriginalStatPoint;

        BaseMaxHp = player.MaxHp.Value;
        BaseDamage = player.DamageValue.Value;

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
    }
    
    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Close()
    {
        base.Close();
        ResetUnappliedChange();

    }

    private void OnConnectButton()
    {
        hpUpgradeButton.onClick.AddListener(OnHpUpgrade);
        hpDowngradeButton.onClick.AddListener(OnHpDowngrade);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgrade);
        damageDowngradeButton.onClick.AddListener(OnDamageDowngrade);
        applyButton.onClick.AddListener(ApplyStatsToPlayer);
    }
    private void OffConnectButton()
    {
        hpUpgradeButton.onClick.RemoveListener(OnHpUpgrade);
        hpDowngradeButton.onClick.RemoveListener(OnHpDowngrade);
        damageUpgradeButton.onClick.RemoveListener(OnDamageUpgrade);
        damageDowngradeButton.onClick.RemoveListener(OnDamageDowngrade);
        applyButton.onClick.RemoveListener(ApplyStatsToPlayer);
    }

    private void UpdateUI()
    {
        hpLevelText.text = $"lv.{TempHpLevel}";
        damageLevelText.text = $"lv.{TempDamageLevel}";

        int hpIncrease = Mathf.RoundToInt(BaseMaxHp * Amount * (TempHpLevel - AppliedHpLevel));
        hpFigure.text = $"+{hpIncrease}";

        float damageIncrease = (BaseDamage * Amount * (TempDamageLevel - AppliedDamageLevel));
        damageFigure.text = $"+{damageIncrease:f1}";

        remainingPointText.text = $"남은 포인트: {RemainingPoint}";
    }

    void OnHpUpgrade()
    {
        if (RemainingPoint > 0)
        {
            TempHpLevel++;
            RemainingPoint--;
            UpdateUI();
        }
    }

    void OnHpDowngrade()
    {
        if (TempHpLevel > AppliedHpLevel)
        {
            TempHpLevel--;
            RemainingPoint++;
            UpdateUI();
        }
    }

    void OnDamageUpgrade()
    {
        if (RemainingPoint > 0)
        {
            TempDamageLevel++;
            RemainingPoint--;
            UpdateUI();
        }
    }

    void OnDamageDowngrade()
    {
        if (TempDamageLevel > AppliedDamageLevel)
        {
            TempDamageLevel--;
            RemainingPoint++;
            UpdateUI();
        }
    }

    private void ApplyStatsToPlayer()
    {
        var player = ServerManager.Instance.ThisPlayer;

        // 차이 계산
        int hpDiff = TempHpLevel - AppliedHpLevel;
        int damageDiff = TempDamageLevel - AppliedDamageLevel;

        // 증가량 계산
        int hpIncrease = Mathf.RoundToInt(BaseMaxHp * Amount * hpDiff);
        int damageIncrease = Mathf.RoundToInt(BaseDamage * Amount * damageDiff);

        // 능력치 반영
        player.MaxHp.Value = hpIncrease + BaseMaxHp;
        player.DamageValue.Value = damageIncrease + BaseDamage;

        // 스텟 포인트 반영
        OriginalStatPoint = RemainingPoint;
        AppliedHpLevel = TempHpLevel;
        AppliedDamageLevel = TempDamageLevel;
        player.StatPoint = RemainingPoint;

        UpdateUI();
    }

    private void ResetUnappliedChange()
    {
        RemainingPoint = OriginalStatPoint;

        TempHpLevel = AppliedHpLevel;
        TempDamageLevel = AppliedDamageLevel;
        UpdateUI();
    }

}
