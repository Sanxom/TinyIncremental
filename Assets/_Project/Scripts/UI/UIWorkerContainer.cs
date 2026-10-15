using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWorkerContainer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image profileImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject lockedOverlay;
    private HRManager hrManager;

    [Header("Buttons")]
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button videoUnlockButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button videoUpgradeButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI unlockPriceText;
    [SerializeField] private TextMeshProUGUI upgradePriceText;

    [Header("Settings")]
    [SerializeField] private UIWorkerStat[] statArray;
    [SerializeField] private int numOfStats = 3;
    private int level;

    [Header("Data")]
    private WorkerDataSO workerData;

    public string WorkerName => workerData.Name;

    public void Init(HRManager hrManager, WorkerDataSO workerData, int workerLevel)
    {
        this.hrManager = hrManager;
        this.workerData = workerData;
        profileImage.sprite = workerData.ProfilePicture;
        nameText.text = workerData.Name;

        InitButtonCallbacks();

        level = workerLevel;

        UpdateButtonVisuals();

        Vector3Int statLevels = GetStatLevelsFromLevel(Mathf.Max(workerData.InitialLevel, level));

        statArray[0].Init(workerData.MaxSpeed, statLevels.x);
        statArray[1].Init(workerData.MaxCapacity, statLevels.y);
        statArray[2].Init(workerData.MaxRevenue, statLevels.z);

        if (level < 0)
        {
            UpdateUnlockButton();
            return;
        }

        Unlock();
    }

    public void Unlock()
    {
        lockedOverlay.SetActive(false);
        level = Mathf.Max(workerData.InitialLevel, level);

        UpdateButtonVisuals();
    }

    public void LevelUp()
    {
        int statIndex = level % statArray.Length;
        statArray[statIndex].Increment();

        level++;



        UpdateButtonVisuals();
    }

    public void UpdateButtonVisuals()
    {
        if (lockedOverlay.activeInHierarchy)
        {
            UpdateUnlockButton();
            return;
        }

        int upgradePrice = HRManager.GetWorkerUpgradePriceFromLevel(level);
        upgradePriceText.text = $"<sprite=0> {upgradePrice}";

        if (level >= workerData.MaxSpeed * numOfStats)
            upgradePriceText.text = "Max";

        upgradeButton.interactable = CurrencyManager.Instance.HasEnoughCurrency(upgradePrice)
            && level < workerData.MaxSpeed * numOfStats;

        videoUpgradeButton.gameObject.SetActive(level < workerData.MaxSpeed * numOfStats);
    }

    private Vector3Int GetStatLevelsFromLevel(int level)
    {
        int speedLevel = Mathf.CeilToInt((float)level / numOfStats);
        int capacityLevel = Mathf.CeilToInt(Mathf.Max(0f, level - 1) / numOfStats);
        int revenueLevel = Mathf.CeilToInt(Mathf.Max(0f, level - 2) / numOfStats);

        return new Vector3Int(speedLevel, capacityLevel, revenueLevel);
    }

    private void InitButtonCallbacks()
    {
        unlockButton.onClick.AddListener(() => hrManager.OnContainerUnlockButtonClicked(this));
        videoUnlockButton.onClick.AddListener(() => hrManager.OnContainerVideoUnlockButtonClicked(this));
        upgradeButton.onClick.AddListener(() => hrManager.OnContainerUpgradeButtonClicked(this));
        videoUpgradeButton.onClick.AddListener(() => hrManager.OnContainerVideoUpgradeButtonClicked(this));
    }

    private void UpdateUnlockButton()
    {
        int containerIndex = transform.GetSiblingIndex();

        // TODO: Remove this if you don't want the first worker to be free.
        //       Also remove from HRManager.OnContainerUnlockButtonClicked()
        if (containerIndex <= 0)
        {
            unlockPriceText.text = "FREE";
            unlockButton.interactable = true;
            return;
        }

        unlockPriceText.text = $"<sprite=0> {workerData.UnlockPrice}";
        unlockButton.interactable = CurrencyManager.Instance.HasEnoughCurrency(workerData.UnlockPrice);
    }
}