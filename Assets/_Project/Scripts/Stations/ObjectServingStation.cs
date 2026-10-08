using System;
using UnityEngine;

[RequireComponent(typeof(ObjectServingCustomerManager))]
[RequireComponent(typeof(GuidGenerator))]
public class ObjectServingStation : MonoBehaviour
{
    [Header("Components")]
    private ObjectServingCustomerManager objectServingCustomerManager;
    private GuidGenerator guidGenerator;

    [Header("Elements")]
    [SerializeField] private ObjectDropZone dropZone;
    [SerializeField] private TableManager tableManager;
    [SerializeField] private TaskRequester taskRequester;
    [SerializeField] private Transform workerServingTargetPoint;

    [Header("Settings")]
    [SerializeField] private SpawnableObject objectServedPrefab;
    [SerializeField] private float servingDelay;
    [SerializeField] private int minObjectsToRequestFillOrServe = 4;
    private float servingTimer;
    private int workerCount;

    [Header("Request Timer")]
    [SerializeField] private float requestCheckDelay;
    private float requestCheckTimer;

    private void Awake()
    {
        objectServingCustomerManager = GetComponent<ObjectServingCustomerManager>();
        guidGenerator = GetComponent<GuidGenerator>();
    }

    private void Update()
    {
        HandleRequestTimer();
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

        workerCount--;
        workerCount = Mathf.Max(0, workerCount);
    }

    private ObjectPosition GetFirstObjectPosition()
    {
        return dropZone.GetFirstObjectPosition();
    }

    private SpawnableObject Pop()
    {
        return dropZone.Pop();
    }

    private bool HasEnoughObjects()
    {
        return dropZone.ObjectCount >= minObjectsToRequestFillOrServe;
    }

    private bool CanSendServeCustomersRequest()
    {
        return workerCount <= 0 && objectServingCustomerManager.IsCustomerReadyToTakeObject() && HasEnoughObjects();
    }

    private void DequeueCustomer(Customer customerToServe)
    {
        if (!tableManager.IsAnyTableAvailable())
            return;

        objectServingCustomerManager.Dequeue();
        tableManager.HandleCustomerServed(customerToServe);
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
        {
            DequeueCustomer(objectServingCustomerManager.PeekFirstCustomer());
            servingTimer = 0f;
            return;
        }

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

    private void HandleRequestTimer()
    {
        if (requestCheckTimer < requestCheckDelay)
        {
            requestCheckTimer += Time.deltaTime;
            return;
        }

        CheckRequests();
        requestCheckTimer = 0f;
    }

    private void CheckRequests()
    {
        // Do we have enough objects?
        // If not, send a fillPlateau request
        if (!HasEnoughObjects())
        {
            // Emit request
            taskRequester.CreateTaskRequest(
                new FillStationPlateauRequest(guidGenerator.GUID, objectServedPrefab, dropZone.WorkerTargetPosition));
        }

        if (CanSendServeCustomersRequest())
        {
            taskRequester.CreateTaskRequest(
                new ServeCustomersRequest(guidGenerator.GUID, workerServingTargetPoint.position, dropZone));
        }

        // Can we serve Customers?
        // If so, send a serve Customers request
    }
}