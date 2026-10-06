using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPosition : MonoBehaviour
{
    [Header("Elements")]
    private SpawnableObject objectInThisPosition;

    [Header("Settings")]
    private bool isEmpty;
    public bool IsEmpty => isEmpty;

    private void Awake()
    {
        isEmpty = true;
    }

    public void Push(SpawnableObject objectInstance)
    {
        objectInThisPosition = objectInstance;

        objectInstance.transform.SetParent(transform);
        objectInstance.transform.localPosition = Vector3.zero;

        isEmpty = false;
    }

    public SpawnableObject Pop()
    {
        isEmpty = true;

        SpawnableObject objectToReturn = objectInThisPosition;
        objectInThisPosition = null;

        return objectToReturn;
    }
}