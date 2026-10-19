using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tabsil.Sijil;
using System;

public class UpgradeDeskStation : MonoBehaviour, IWantToBeSaved
{
    [Header("Elements")]
    [SerializeField] private PlayerStatsHandler playerStatsHandler;
    [SerializeField] private CanvasGroup upgradePanel;
    [SerializeField] private UIPlayerUpgradeContainer upgradeContainerPrefab;
    [SerializeField] private Transform upgradeContainerParent;
    private List<UIPlayerUpgradeContainer> upgradeContainerList = new();

    [Header("Settings")]
    private bool hasEntered;
    private bool isLoaded;

    [Header("Data")]
    [SerializeField] private BaseCharacterStatsDataSO playerBaseStats;
    private int[] statLevelArray;
    private const string STAT_LEVEL_KEY = "PLAYER_STAT_LEVEL_";
    private const int NUM_OF_PLAYER_STATS = 3;

    private void Awake()
    {
        CurrencyManager.OnUpdated += OnCurrencyUpdated;
    }

    private void Start()
    {
        if (!isLoaded)
            Load();
    }

    private void OnTriggerStay(Collider other)
    {
        if (upgradePanel.alpha > 0 || hasEntered) 
            return;

        if (!other.TryGetComponent(out PlayerController playerController))
            return;

        if (playerController.IsMoving())
            return;

        hasEntered = true;

        Show();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController playerController))
            return;

        hasEntered = false;
    }

    private void OnDestroy()
    {
        CurrencyManager.OnUpdated -= OnCurrencyUpdated;
    }

    // TODO: Use a robust formula for determining the upgrade price rather than this hard-coded value
    public int GetUpgradePrice(int statLevel)
    {
        return (statLevel + 1) * 50;
    }

    public void Load()
    {
        isLoaded = true;

        // TODO: This needs to be more efficient rather than setting in the code
        statLevelArray = new int[NUM_OF_PLAYER_STATS];

        for (int i = 0; i < statLevelArray.Length; i++)
        {
            if (Sijil.TryLoad(this, $"{STAT_LEVEL_KEY}{i}", out object _statLevel))
                statLevelArray[i] = (int)_statLevel;
        }

        GenerateUpgradeContainers();

        playerStatsHandler.UpdateStats(statLevelArray);
    }

    public void Save()
    {
        for (int i = 0; i < statLevelArray.Length; i++)
            Sijil.Save(this, $"{STAT_LEVEL_KEY}{i}", statLevelArray[i]);
    }

    public void Show()
    {
        upgradePanel.Show();

        OnCurrencyUpdated();
    }

    public void Hide()
    {
        upgradePanel.Hide();
    }

    private void GenerateUpgradeContainers()
    {
        for (int i = 0; i < statLevelArray.Length; i++)
        {
            int statLevel = statLevelArray[i];

            UIPlayerUpgradeContainer containerInstance = Instantiate(upgradeContainerPrefab, upgradeContainerParent);
            containerInstance.Init(this, playerBaseStats, i, statLevel);

            upgradeContainerList.Add(containerInstance);
        }
    }

    public void OnContainerUpgradeButtonClicked(UIPlayerUpgradeContainer container)
    {
        int containerIndex = container.transform.GetSiblingIndex();

        int upgradePrice = GetUpgradePrice(statLevelArray[containerIndex]);

        CurrencyManager.Instance.RemoveCurrency(upgradePrice);

        statLevelArray[containerIndex]++;
        container.LevelUp();

        playerStatsHandler.UpdateStats(statLevelArray);

        Save();
    }

    public void OnContainerVideoUpgradeButtonClicked(UIPlayerUpgradeContainer container)
    {
        int containerIndex = container.transform.GetSiblingIndex();
        statLevelArray[containerIndex]++;
        container.LevelUp();

        playerStatsHandler.UpdateStats(statLevelArray);

        Save();
    }

    private void OnCurrencyUpdated()
    {
        for (int i = 0; i < upgradeContainerList.Count; i++)
            upgradeContainerList[i].UpdateButtonVisuals();
    }
}