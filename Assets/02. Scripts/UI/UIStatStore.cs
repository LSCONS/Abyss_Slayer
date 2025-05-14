using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatStore : UIPopup
{
    [SerializeField] TextMeshProUGUI hpLevelText;
    [SerializeField] TextMeshProUGUI damageLevelText;

    [SerializeField] TextMeshProUGUI hpFigure;
    [SerializeField] TextMeshProUGUI damageFigure;

    [SerializeField] TextMeshProUGUI remainingPoint;

    [SerializeField] Button hpUpgradeButton;
    [SerializeField] Button hpDowngradeButton;

    [SerializeField] Button damageUpgradeButton;
    [SerializeField] Button damageDowngradeButton;

    [SerializeField] Button applyButton;


    private int hpLevel = 0;
    private int damageLevel = 0;
    private int remainingPointValue;

    private int baseMaxHp;
    private int baseDamage;

    public override async void Init()
    {
        base.Init();
        Player player = await ServerManager.Instance.WaitForThisPlayerAsync();
        remainingPointValue = player.StatPoint;

        baseMaxHp = player.MaxHp.Value;
        baseDamage = player.DamageValue.Value;

        hpUpgradeButton.onClick.AddListener(OnHpUpgrade);
        hpDowngradeButton.onClick.AddListener(OnHpDowngrade);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgrade);
        damageDowngradeButton.onClick.AddListener(OnDamageDowngrade);
        applyButton.onClick.AddListener(ApplyStatsToPlayer);

        UpdateUI();
    }
    private void OnDestroy()
    {
        hpUpgradeButton.onClick.RemoveListener(OnHpUpgrade);
        hpDowngradeButton.onClick.RemoveListener(OnHpDowngrade);
        damageUpgradeButton.onClick.RemoveListener(OnDamageUpgrade);
        damageDowngradeButton.onClick.RemoveListener(OnDamageDowngrade);
        applyButton.onClick.RemoveListener(ApplyStatsToPlayer);

    }

    private void UpdateUI()
    {
        hpLevelText.text = $"lv. {hpLevel}";
        damageLevelText.text = $"lv. {damageLevel}";

        int hpIncrease = Mathf.RoundToInt(baseMaxHp * 0.1f * hpLevel);
        hpFigure.text = $"+{hpIncrease}";

        int damageIncrease = 0;
        for (int i = 0; i < damageLevel; i++)
        {
            damageIncrease += Mathf.Max(1, Mathf.RoundToInt(baseDamage * 0.1f));
        }
        damageFigure.text = $"+{damageIncrease}";


        remainingPoint.text = $"남은 포인트: {remainingPointValue}";
    }

    void OnHpUpgrade()
    {
        if (remainingPointValue > 0)
        {
            hpLevel++;
            remainingPointValue--;
            UpdateUI();
        }
    }

    void OnHpDowngrade()
    {
        if (hpLevel > 0)
        {
            hpLevel--;
            remainingPointValue++;
            UpdateUI();
        }
    }

    void OnDamageUpgrade()
    {
        if (remainingPointValue > 0)
        {
            damageLevel++;
            remainingPointValue--;
            UpdateUI();
        }
    }

    void OnDamageDowngrade()
    {
        if (damageLevel > 0)
        {
            damageLevel--;
            remainingPointValue++;
            UpdateUI();
        }
    }

    void ApplyStatsToPlayer()
    {
        var player = ServerManager.Instance.ThisPlayer;

        int hpIncrease = hpLevel > 0
            ? Mathf.RoundToInt(baseMaxHp * 0.1f * hpLevel)
            : 0;

        int damageIncrease = 0;
        for (int i = 0; i < damageLevel; i++)
        {
            damageIncrease += Mathf.Max(1, Mathf.RoundToInt(baseDamage * 0.1f));
        }
        player.DamageValue.Value = baseDamage + damageIncrease;

        player.MaxHp.Value = baseMaxHp + hpIncrease;
        player.DamageValue.Value = baseDamage + damageIncrease;
    }

}
