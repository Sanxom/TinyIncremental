using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plateau : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform objectPositionsParent;

    [Header("Settings")]
    [SerializeField] private int maxCapacity;

    private float positionsYOffset;
    private bool isFull;

    public bool IsFull => isFull;

    private void Awake()
    {
        isFull = false;
    }

    public void Push(SpawnableObject objectInstance)
    {
        ObjectPosition objectPosition = GetFirstEmptyObjectPosition();
        objectPosition.Push(objectInstance);

        RearrangeObjectPositions(objectInstance);

        if (GetFirstEmptyObjectPosition() == null)
        {
            if (objectPositionsParent.childCount < maxCapacity)
                CreateNewObjectPosition();
            else
                isFull = true;
        }
    }

    private void CreateNewObjectPosition()
    {
        ObjectPosition objectPositionInstance = new GameObject($"Object Position {objectPositionsParent.childCount}").AddComponent<ObjectPosition>();
        objectPositionInstance.transform.SetParent(objectPositionsParent);

        int bottomChildIndex = objectPositionInstance.transform.GetSiblingIndex() - 1;
        objectPositionInstance.transform.SetLocalPositionAndRotation(objectPositionsParent.GetChild(bottomChildIndex).localPosition + Vector3.up * positionsYOffset, Quaternion.identity);
        isFull = false;
    }

    private void RearrangeObjectPositions(SpawnableObject objectInstance)
    {
        positionsYOffset = objectInstance.CleanYOffsetOnPlateau;

        for (int i = 0; i < objectPositionsParent.childCount; i++)
        {
            objectPositionsParent.GetChild(i).localPosition = i * positionsYOffset * Vector3.up;
        }
    }

    private ObjectPosition GetFirstEmptyObjectPosition()
    {
        for (int i = 0; i < objectPositionsParent.childCount; i++)
        {
            if (!objectPositionsParent.GetChild(i).TryGetComponent(out ObjectPosition objectPosition))
                continue;

            if (objectPosition.IsEmpty)
                return objectPosition;
        }

        return null;
    }
}