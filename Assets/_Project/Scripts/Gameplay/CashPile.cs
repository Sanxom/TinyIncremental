using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tabsil.Sijil;

[RequireComponent(typeof(GuidGenerator))]
public class CashPile : MonoBehaviour, IWantToBeSaved
{
    [Header("Components")]
    private GuidGenerator guidGenerator;

    [Header("Elements")]
    [SerializeField] private ObjectPooler cashObjectPool;
    [SerializeField] private GameObject cashPrefab;

    [Header("Settings")]
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector3 gridSpacing;
    [SerializeField] private int valuePerCashObject = 2;
    private Vector3[] basePositionArray;
    private int cashAmountInPileToBeSaved;
    private bool isLoaded;

    private void Awake()
    {
        guidGenerator = GetComponent<GuidGenerator>();

        StoreBasePositionsInArray();
    }

    private void Start()
    {
        if (!isLoaded)
            Load();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player))
            return;

        AnimateCashToPlayer(player.transform);

        cashAmountInPileToBeSaved = 0;

        Save();
    }

    public void Load()
    {
        isLoaded = true;

        if (!Sijil.TryLoad(this, guidGenerator.GUID, out object savedCashPileAmount))
            return;

        GenerateCash((int)savedCashPileAmount, false);
    }

    public void Save()
    {
        Sijil.Save(this, guidGenerator.GUID, cashAmountInPileToBeSaved);
    }

    public void GenerateCash(int amount, bool wantToSave = true)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 targetPosition = GetTargetGridPosition(i + cashAmountInPileToBeSaved);
            GameObject cashPrefab = cashObjectPool.GetPooledObject();
            cashPrefab.transform.SetParent(transform);
            cashPrefab.transform.SetPositionAndRotation(targetPosition, Quaternion.identity);
            cashPrefab.SetActive(true);
        }

        cashAmountInPileToBeSaved += amount;

        if (wantToSave)
            Save();
    }

    private void AnimateCashToPlayer(Transform playerTransform)
    {
        float duration = 2f;
        float delayStep = duration / transform.childCount;

        delayStep = Mathf.Min(delayStep, 0.01f);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform cash = transform.GetChild(i);
            float delay = (transform.childCount - 1 - i) * delayStep;

            delay = Mathf.Min(delay, duration);
            ArcAnimator.Instance.Animate(cash, playerTransform, 0.2f, delay, 3f, () => HandleCashMovedAlongArc(cash.gameObject));
        }
    }

    private void HandleCashMovedAlongArc(GameObject cashObject)
    {
        CurrencyManager.Instance.AddCurrency(valuePerCashObject);
        cashObjectPool.ReturnObjectToPool(cashObject);
    }

    private Vector3 GetTargetGridPosition(int index)
    {
        int elevationIndex = index / basePositionArray.Length;
        float y = elevationIndex * gridSpacing.y;

        int basePositionIndex = index % basePositionArray.Length;
        return basePositionArray[basePositionIndex] + Vector3.up * y;
    }

    private void StoreBasePositionsInArray()
    {
        basePositionArray = new Vector3[gridSize.x * gridSize.y];

        Vector3 startPosition = transform.position
            - (0.5f * gridSize.x * gridSpacing.x * Vector3.right)
            - (0.5f * gridSize.y * gridSpacing.z * Vector3.forward);

        startPosition += gridSpacing * 0.5f;

        for (int z = 0; z < gridSize.y; z++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 targetPosition = startPosition + gridSpacing.x * x * Vector3.right + gridSpacing.z * z * Vector3.forward;
                int i = x + z * gridSize.x;
                basePositionArray[i] = targetPosition;
            }
        }
    }
}