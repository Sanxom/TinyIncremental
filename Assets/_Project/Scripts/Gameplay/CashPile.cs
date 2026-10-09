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
    private Vector3[] basePositionArray;

    private void Awake()
    {
        StoreBasePositionsInArray();
    }

    [NaughtyAttributes.Button]
    private void GenerateOneCash()
    {
        GenerateCash(1);
    }

    public void GenerateCash(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // TODO: Create an ObjectPool for the cash, and if you want a different parent for that pool, reference THAT transform here.
            Vector3 targetPosition = GetTargetGridPosition(transform.childCount);
            Instantiate(cashPrefab, targetPosition, Quaternion.identity, transform);
        }
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