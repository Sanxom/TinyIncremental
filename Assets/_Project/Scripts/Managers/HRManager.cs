using System;
using System.Collections;
using System.Collections.Generic;
using Tabsil.Sijil;
using UnityEngine;

public class HRManager : MonoBehaviour, IWantToBeSaved
{
    [Header("Elements")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private UIWorkerContainer uiWorkerContainerPrefab;
    [SerializeField] private Transform workerContainerParent;

    [Header("Data")]
    [SerializeField] private WorkerDataSO[] workerDataArray;
    private List<UIWorkerContainer> workerContainerList = new();
    private int[] workerLevelArray;
    private const string WORKER_LEVEL_KEY = "WORKER_LEVEL_";

    [Header("Settings")]
    private static readonly int firstUpgradeCost = 10;
    private static readonly int levelUpCostMultiplier = 20;
    public bool IsPanelVisible => canvasGroup.alpha > 0f;

    private void Awake()
    {
        CurrencyManager.OnUpdated += OnCurrencyUpdated;
    }

    private void OnDestroy()
    {
        CurrencyManager.OnUpdated -= OnCurrencyUpdated;
    }

    public static int GetWorkerUpgradePriceFromLevel(int level)
    {
        return firstUpgradeCost + Mathf.Max(level, 0) * levelUpCostMultiplier;
    }

    public void OnContainerUnlockButtonClicked(UIWorkerContainer uiWorkerContainer)
    {
        int workerIndex = uiWorkerContainer.transform.GetSiblingIndex();

        // TODO: Remove this if-statement if you don't want the first worker to be free.
        //       Also remove from UIWorkerContainer.UpdateUnlockButton()
        if (workerIndex <= 0)
        {
            workerLevelArray[workerIndex] = workerDataArray[workerIndex].InitialLevel;
            uiWorkerContainer.Unlock();
            WorkerManager.Instance.SpawnWorker(workerDataArray[workerIndex], workerLevelArray[workerIndex]);
            Save();
        }
        else
        {
            workerLevelArray[workerIndex] = workerDataArray[workerIndex].InitialLevel;
            CurrencyManager.Instance.AddCurrency(-workerDataArray[workerIndex].UnlockPrice);
            uiWorkerContainer.Unlock();
            WorkerManager.Instance.SpawnWorker(workerDataArray[workerIndex], workerLevelArray[workerIndex]);
            Save();
        }
    }

    public void OnContainerVideoUnlockButtonClicked(UIWorkerContainer uiWorkerContainer)
    {
        int workerIndex = uiWorkerContainer.transform.GetSiblingIndex();

        workerLevelArray[workerIndex] = workerDataArray[workerIndex].InitialLevel;

        uiWorkerContainer.Unlock();

        WorkerManager.Instance.SpawnWorker(workerDataArray[workerIndex], workerLevelArray[workerIndex]);

        Save();
    }

    public void OnContainerUpgradeButtonClicked(UIWorkerContainer uiWorkerContainer)
    {
        int workerIndex = uiWorkerContainer.transform.GetSiblingIndex();

        CurrencyManager.Instance.AddCurrency(-GetWorkerUpgradePriceFromLevel(workerLevelArray[workerIndex]));

        workerLevelArray[workerIndex]++;
        uiWorkerContainer.LevelUp();

        WorkerManager.Instance.LevelUpWorker(uiWorkerContainer.WorkerName);

        Save();
    }

    public void OnContainerVideoUpgradeButtonClicked(UIWorkerContainer uiWorkerContainer)
    {
        int workerIndex = uiWorkerContainer.transform.GetSiblingIndex();

        workerLevelArray[workerIndex]++;
        uiWorkerContainer.LevelUp();

        WorkerManager.Instance.LevelUpWorker(uiWorkerContainer.WorkerName);

        Save();
    }

    public void Load()
    {
        workerLevelArray = new int[workerDataArray.Length];

        for (int i = 0; i < workerLevelArray.Length; i++)
        {
            if (Sijil.TryLoad(this, $"{WORKER_LEVEL_KEY}{i}", out object _workerLevel))
                workerLevelArray[i] = (int)_workerLevel;
            else
                workerLevelArray[i] = -1;
        }

        GenerateWorkerContainers();

        WorkerManager.Instance.Init(workerDataArray, workerLevelArray);
    }

    public void Save()
    {
        for (int i = 0; i < workerLevelArray.Length; i++)
            Sijil.Save(this, $"{WORKER_LEVEL_KEY}{i}", workerLevelArray[i]);
    }

    public void Show()
    {
        canvasGroup.Show();

        OnCurrencyUpdated();
    }

    public void Hide()
    {
        canvasGroup.Hide();
    }

    private void GenerateWorkerContainers()
    {
        for (int i = 0; i < workerDataArray.Length; i++)
        {
            int workerLevel = workerLevelArray[i];
            // TODO: If you want to add ObjectPooling
            UIWorkerContainer containerInstance = Instantiate(uiWorkerContainerPrefab, workerContainerParent);
            containerInstance.Init(this, workerDataArray[i], workerLevel);
            workerContainerList.Add(containerInstance);
        }
    }

    private void OnCurrencyUpdated()
    {
        for (int i = 0; i < workerContainerList.Count; i++)
            workerContainerList[i].UpdateButtonVisuals();
    }
}