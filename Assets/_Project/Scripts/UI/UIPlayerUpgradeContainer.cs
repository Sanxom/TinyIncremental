using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerUpgradeContainer : MonoBehaviour
{
    [Header("Elements")]
    private UpgradeDeskStation station;

    [Header("Visuals")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image icon;
    [SerializeField] private Transform pipParent;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button videoUpgradeButton;
    [SerializeField] private TextMeshProUGUI upgradePriceText;

    [Header("Data")]
    // TODO: Change this if you want more or less levels
    [SerializeField] private int maxLevel = 5;
    private BaseCharacterStatsDataSO baseStats;
    private int statIndex;
    private int level;


    // TODO: The names and icons are based on having ONLY 3 stats. We'll need to update this to include new stats if we want to add more!
    public void Init(UpgradeDeskStation station, BaseCharacterStatsDataSO baseStats, int statIndex, int statLevel)
    {
        this.station = station;
        this.baseStats = baseStats;
        this.statIndex = statIndex;
        level = statLevel;

        nameText.text = GetStatName(statIndex);
        icon.sprite = GetStatIcon(statIndex);

        level = Mathf.Min(level, maxLevel);

        for (int i = 0; i < level; i++)
            pipParent.GetChild(i).GetComponent<UIUpgradePip>().Activate();

        UpdateButtonVisuals();

        InitButtonCallbacks();
    }

    public void LevelUp()
    {
        pipParent.GetChild(level).GetComponent<UIUpgradePip>().Activate();
        level++;

        UpdateButtonVisuals();
    }

    public void UpdateButtonVisuals()
    {
        int upgradePrice = station.GetUpgradePrice(level);

        if (level >= maxLevel)
            upgradePriceText.text = "MAX";
        else
            upgradePriceText.text = $"<sprite=0> {upgradePrice}";

        upgradeButton.interactable = CurrencyManager.Instance.HasEnoughCurrency(upgradePrice) && level < maxLevel;
        videoUpgradeButton.gameObject.SetActive(level < maxLevel);
        // TODO: Change the videoUpgradeButton interactability here to a more robust formula if you don't want people to cap fast with just watching videos
        videoUpgradeButton.interactable = level < maxLevel;
    }

    private Sprite GetStatIcon(int statIndex) => statIndex switch
    {
        0 => baseStats.SpeedIcon,
        1 => baseStats.CapacityIcon,
        2 => baseStats.RevenueIcon,
        _ => null,
    };

    private string GetStatName(int statIndex) => statIndex switch
    {
        0 => "Speed",
        1 => "Capacity",
        2 => "Revenue",
        _ => "",
    };

    private void InitButtonCallbacks()
    {
        upgradeButton.onClick.AddListener(() => station.OnContainerUpgradeButtonClicked(this));
        videoUpgradeButton.onClick.AddListener(() => station.OnContainerVideoUpgradeButtonClicked(this));
    }
}