using System;
using System.Collections.Generic;
using UnityEngine;

public class Plateau : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform objectPositionsParent;

    [Header("Settings")]
    [SerializeField] private int maxCapacity;

    private SpawnableObject lastObjectPushed;
    private float positionsYOffset;
    private bool isFull;
    private bool isDirty;

    public bool IsFull => isFull;
    public bool IsDirty => isDirty;
    public bool IsEmpty => GetFirstObjectPosition() == null;

    private void Awake()
    {
        isFull = false;
    }

    public SpawnableObject[] PopAll()
    {
        List<SpawnableObject> objectList = new();

        for (int i = 0; i < objectPositionsParent.childCount; i++)
        {
            ObjectPosition objectPosition = objectPositionsParent.GetChild(i).GetComponent<ObjectPosition>();

            if (objectPosition.IsEmpty)
                continue;

            objectList.Add(objectPosition.Pop());
        }

        isFull = false;
        isDirty = false;

        return objectList.ToArray();
    }

    public SpawnableObject Pop()
    {
        ObjectPosition objectPosition = GetLastObjectPosition();

        if (objectPosition == null)
            return null;

        isFull = false;

        return objectPosition.Pop();
    }

    public ObjectPosition GetFirstObjectPosition()
    {
        for (int i = 0; i < objectPositionsParent.childCount; i++)
        {
            if (!objectPositionsParent.GetChild(i).TryGetComponent(out ObjectPosition objectPosition))
                continue;

            if (!objectPosition.IsEmpty)
                return objectPosition;
        }

        return null;
    }

    public int GetObjectCountInPlateau()
    {
        int counter = 0;

        for (int i = 0; i < objectPositionsParent.childCount; i++)
        {
            if (objectPositionsParent.GetChild(i).GetComponent<ObjectPosition>().IsEmpty)
                continue;

            counter++;
        }

        return counter;
    }

    public void Push(SpawnableObject objectInstance)
    {
        if (objectInstance.IsDirty)
            isDirty = true;

        if (isDirty && isFull)
            CreateNewObjectPosition();

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
        else
        {
            int occupiedPositions = 0;
            for (int i = 0; i < objectPositionsParent.childCount; i++)
            {
                ObjectPosition newObjectPosition = objectPositionsParent.GetChild(i).GetComponent<ObjectPosition>();
                if (!newObjectPosition.IsEmpty)
                    occupiedPositions++;
                if (occupiedPositions >= maxCapacity)
                {
                    isFull = true;
                    break;
                }
            }
        }

        lastObjectPushed = objectInstance;
    }

    public void MarkAsDirty()
    {
        for (int i = 0; i < objectPositionsParent.childCount; i++)
        {
            ObjectPosition objectPosition = objectPositionsParent.GetChild(i).GetComponent<ObjectPosition>();

            if (objectPosition.IsEmpty) 
                continue;

            objectPosition.DisplayObject();
            objectPosition.MarkAsDirty();
            isDirty = true;
        }

        RearrangeObjectPositions(lastObjectPushed);
    }

    public void HideObject()
    {
        for (int i = objectPositionsParent.childCount - 1; i >= 0; i--)
        {
            ObjectPosition objectPosition = objectPositionsParent.GetChild(i).GetComponent<ObjectPosition>();

            if (objectPosition.IsEmpty)
                continue;
            if (!objectPosition.IsObjectVisible)
                continue;

            objectPosition.HideObject();
            break;
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

    private ObjectPosition GetLastObjectPosition()
    {
        for (int i = objectPositionsParent.childCount - 1; i >= 0; i--)
        {
            if (!objectPositionsParent.GetChild(i).TryGetComponent(out ObjectPosition objectPosition))
                continue;

            if (!objectPosition.IsEmpty)
                return objectPosition;
        }

        return null;
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
        positionsYOffset = objectInstance.IsDirty ? objectInstance.DirtyYOffsetOnPlateau : objectInstance.CleanYOffsetOnPlateau;

        int hiddenObjectCount = 0;

        for (int i = 0; i < objectPositionsParent.childCount; i++)
        {
            if (!objectPositionsParent.GetChild(i).GetComponent<ObjectPosition>().IsObjectVisible)
                hiddenObjectCount++;

            objectPositionsParent.GetChild(i).localPosition = (i - hiddenObjectCount) * positionsYOffset * Vector3.up;
        }
    }
}