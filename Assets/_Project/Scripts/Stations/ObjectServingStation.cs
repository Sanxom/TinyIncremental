using System;
using UnityEngine;

[RequireComponent(typeof(ObjectServingCustomerManager))]
[RequireComponent(typeof(GuidGenerator))]
public abstract class ObjectServingStation : MonoBehaviour
{
    [Header("Components")]
    private GuidGenerator guidGenerator;
    protected ObjectServingCustomerManager objectServingCustomerManager;

    [Header("Elements")]
    [SerializeField] private ObjectDropZone dropZone;
    [SerializeField] private TaskRequester taskRequester;
    [SerializeField] private Transform workerServingTargetPoint;
    [SerializeField] private CashPile cashPile;

    [Header("Settings")]
    [SerializeField] private SpawnableObject objectServedPrefab;
    [SerializeField] private float servingDelay;
    [SerializeField] private int minObjectsToRequestFillOrServe = 4;
    private int workerCount;
    protected float servingTimer;

    [Header("Request Timer")]
    [SerializeField] private float requestCheckDelay;
    private float requestCheckTimer;

    private void Awake()
    {
        objectServingCustomerManager = GetComponent<ObjectServingCustomerManager>();
        guidGenerator = GetComponent<GuidGenerator>();
        dropZone.SetAcceptedObject(objectServedPrefab);
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
        if (!other.TryGetComponent(out CharacterStats characterStats))
            return;

        if (workerCount > 0)
            HandleObjectServing(characterStats);
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

    protected abstract void DequeueCustomer(Customer customer);

    private void HandleObjectServing(CharacterStats characterStats)
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

        ServeObject(characterStats);
    }

    protected abstract void ServeObject(CharacterStats characterStats);

    protected void ServeCustomer(CharacterStats characterStats)
    {
        Customer customerToServe = objectServingCustomerManager.PeekFirstCustomer();

        // TODO: Use a better formula to be able to generate more cash per serve
        int baseRevenue = 1;
        float revenueMultiplier = MathF.Max(1f, characterStats.Revenue);
        int revenue = Mathf.CeilToInt(baseRevenue * revenueMultiplier);

        cashPile.GenerateCash(revenue);

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
        if (!HasEnoughObjects())
        {
            taskRequester.CreateTaskRequest(
                new FillStationPlateauRequest(guidGenerator.GUID, objectServedPrefab, dropZone.WorkerTargetPosition));
        }

        if (CanSendServeCustomersRequest())
        {
            taskRequester.CreateTaskRequest(
                new ServeCustomersRequest(guidGenerator.GUID, workerServingTargetPoint.position, dropZone));
        }
    }
}