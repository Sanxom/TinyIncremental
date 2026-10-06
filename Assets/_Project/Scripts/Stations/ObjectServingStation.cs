using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectServingCustomerManager))]
public class ObjectServingStation : MonoBehaviour
{
    [Header("Components")]
    private ObjectServingCustomerManager objectServingCustomerManager;

    [Header("Elements")]
    [SerializeField] private ObjectDropZone dropZone;

    [Header("Settings")]
    [SerializeField] private float servingDelay;
    private float servingTimer;
    private int workerCount;

    private void Awake()
    {
        objectServingCustomerManager = GetComponent<ObjectServingCustomerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerDetector playerDetector))
            return;

        workerCount++;
    }

    private void OnTriggerStay(Collider other)
    {
        if (workerCount > 0)
            HandleObjectServing();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerDetector playerDetector))
            return;
        print($"Worker Count Before: {workerCount}");
        Mathf.Max(0, --workerCount);
        print($"Worker Count: {workerCount}");
    }

    private ObjectPosition GetFirstObjectPosition()
    {
        return dropZone.GetFirstObjectPosition();
    }

    private SpawnableObject Pop()
    {
        return dropZone.Pop();
    }

    private void HandleObjectServing()
    {
        if (servingTimer < servingDelay)
        {
            servingTimer += Time.deltaTime;
            return;
        }

        if (!objectServingCustomerManager.IsCustomerReadyToTakeObject())
            return;

        if (GetFirstObjectPosition() == null)
            return;

        if (!objectServingCustomerManager.PeekFirstCustomer().NeedsMoreObjects())
            return;

        ServeObject();
    }

    private void ServeObject()
    {
        servingTimer = 0f;

        Customer customerToServe = objectServingCustomerManager.PeekFirstCustomer();
        SpawnableObject objectToServe = Pop();

        customerToServe.CollectObject(objectToServe);

        if (customerToServe.NeedsMoreObjects())
            return;

        DequeueCustomer(customerToServe);
    }

    private void DequeueCustomer(Customer customerToServe)
    {

    }
}