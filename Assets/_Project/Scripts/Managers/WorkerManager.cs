using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    [Header("Elements")]
    [SerializeField] private List<Worker> workerList = new();
    private List<TaskRequest> pendingRequestList = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        HandleRequests();
    }

    public void RegisterRequest(TaskRequest request)
    {
        pendingRequestList.Add(request);
    }

    private void HandleRequests()
    {
        // Do we have pending requests?

        // Do we have Idle Workers?

        // Assign pending request to the first Idle Worker

        // Remove this request from the pending request
    }
}