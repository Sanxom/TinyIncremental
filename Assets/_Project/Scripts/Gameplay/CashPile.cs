using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashPile : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private GameObject cashPrefab;

    [Header("Settings")]
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector3 gridSpacing;
    [SerializeField] private int valuePerCashObject = 2;
    private Vector3[] basePositionArray;
    private int index;

    private void Awake()
    {
        StoreBasePositionsInArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player))
            return;

        AnimateCashToPlayer(player.transform);

        index = 0;

        // TODO: Save();
    }

    public void GenerateCash(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // TODO: Create an ObjectPool for the cash, and if you want a different parent for that pool, reference THAT transform here.
            Vector3 targetPosition = GetTargetGridPosition(i + index);
            Instantiate(cashPrefab, targetPosition, Quaternion.identity, transform);
        }

        index += amount;
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
        // TODO: Return this cashObject to its ObjectPool
        Destroy(cashObject);
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