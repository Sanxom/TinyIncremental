using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    [Header("Elements")]
    [SerializeField] private List<Worker> workerList = new();
    [SerializeField] private Trash trash;
    [SerializeReference] private List<TaskRequest> pendingRequestList = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        TableManager.OnTableCleaned += OnTableCleaned;
    }

    private void Update()
    {
        HandleRequests();
    }

    private void OnDestroy()
    {
        TableManager.OnTableCleaned -= OnTableCleaned;
    }

    private Worker GetClosestWorkerFromIdleWorkerList(List<Worker> idleWorkerList, Vector3 taskTargetPosition)
    {
        float minDistance = float.MaxValue;
        int workerIndex = -1;

        for (int i = 0; i < idleWorkerList.Count; i++)
        {
            float distance = Vector3.Distance(idleWorkerList[i].transform.position, taskTargetPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                workerIndex = i;
            }
        }

        return idleWorkerList[workerIndex];
    }

    private Vector3 GetTargetPositionFromRequest(TaskRequest request) => request switch
    {
        FillStationPlateauRequest => (request as FillStationPlateauRequest).DropZonePosition,
        ServeCustomersRequest => (request as ServeCustomersRequest).WorkerTargetPosition,
        CleanTableRequest => (request as CleanTableRequest).Table.WorkerTargetPosition,
        _ => Vector3.zero,
    };

    private TaskRequest GetHighestPriorityRequest()
    {
        int maxPriority = int.MinValue;
        int requestIndex = -1;

        for (int i = 0; i < pendingRequestList.Count; i++)
        {
            if (pendingRequestList[i].Priority > maxPriority)
            {
                maxPriority = pendingRequestList[i].Priority;
                requestIndex = i;
            }
        }

        return pendingRequestList[requestIndex];
    }

    public void RegisterRequest(TaskRequest request)
    {
        pendingRequestList.Add(request);
    }

    private void HandleRequest(TaskRequest request, Worker worker)
    {
        switch (request)
        {
            case FillStationPlateauRequest:
                HandleFillStationPlateauRequest(request, worker);
                break;
            case ServeCustomersRequest:
                HandleServeCustomersRequest(request, worker);
                break;
            case CleanTableRequest:
                HandleCleanTableRequest(request, worker);
                break;
            case IdleRequest:
                HandleIdleRequest(request, worker);
                break;
            default:
                break;
        }
    }

    private void HandleIdleRequest(TaskRequest request, Worker worker)
    {
        IdleTask task = new(worker, (request as IdleRequest).TargetPosition, request);
        worker.AssignTask(task);
    }

    private void HandleCleanTableRequest(TaskRequest request, Worker worker)
    {
        CleanTableTask task = new(worker, (request as CleanTableRequest).Table, trash, request);
        worker.AssignTask(task);
    }

    private void HandleServeCustomersRequest(TaskRequest request, Worker worker)
    {
        ServeCustomersTask task = new(worker, (request as ServeCustomersRequest).WorkerTargetPosition, request);
        worker.AssignTask(task);
    }

    private void HandleFillStationPlateauRequest(TaskRequest request, Worker worker)
    {
        FillStationPlateauRequest fillRequest = request as FillStationPlateauRequest;

        ObjectSpawnerStation[] objectSpawnerStationArray = FindObjectsByType<ObjectSpawnerStation>(FindObjectsSortMode.None);

        if (objectSpawnerStationArray.Length <= 0)
        {
            Debug.LogError("WorkerManager: No SpawnerStation found!");
            return;
        }

        List<ObjectSpawnerStation> potentialSpawnerStationList = new();

        for (int i = 0; i < objectSpawnerStationArray.Length; i++)
            if (objectSpawnerStationArray[i].ObjectType == fillRequest.SpawnObject.GetType())
                potentialSpawnerStationList.Add(objectSpawnerStationArray[i]);

        if (potentialSpawnerStationList.Count <= 0)
        {
            Debug.LogError("WorkerManager: No PotentialObjectSpawnerStations found!");
            return;
        }

        ObjectSpawnerStation randomObjectSpawnerStation = potentialSpawnerStationList.ToArray().GetRandom();

        FillStationPlateauTask fillTask = new(worker, randomObjectSpawnerStation.WorkerTargetPosition, fillRequest.DropZonePosition, request);

        worker.AssignTask(fillTask);
    }

    private void HandleRequests()
    {
        // Do we have pending requests?
        if (pendingRequestList.Count <= 0)
            return;

        // Do we have Idle Workers?
        List<Worker> idleWorkerList = new();
        for (int i = 0; i < workerList.Count; i++)
        {
            if (workerList[i].CurrentTask != null)
                continue;
            idleWorkerList.Add(workerList[i]);
        }

        if (idleWorkerList.Count <= 0)
        {
            HandleNoIdleWorkersFound();
            return;
        }

        TaskRequest highestPriorityRequest = GetHighestPriorityRequest();
        Vector3 taskTargetPosition = GetTargetPositionFromRequest(highestPriorityRequest);
        Worker closestWorker = GetClosestWorkerFromIdleWorkerList(idleWorkerList, taskTargetPosition);
        HandleRequest(highestPriorityRequest, closestWorker);
        pendingRequestList.Remove(highestPriorityRequest);
    }

    private void HandleNoIdleWorkersFound()
    {
        for (int i = 0; i < pendingRequestList.Count; i++)
        {
            TaskRequest pendingRequest = pendingRequestList[i];
            bool workerFound = false;

            for (int j = 0; j < workerList.Count; j++)
            {
                Worker worker = workerList[j];

                if (!worker.CanCancelTask)
                    continue;
                // At this point, Worker can cancel their Task
                if (pendingRequest.Priority > worker.CurrentTask.Request.Priority)
                {
                    pendingRequestList.Add(worker.CurrentTask.Request);
                    HandleRequest(pendingRequest, worker);

                    workerFound = true;
                    break;
                }
            }

            if (workerFound)
            {
                pendingRequestList.Remove(pendingRequest);
                break;
            }
        }
    }

    private void OnTableCleaned(TableSet table, HoldUsedObjectAbility holdUsedObjectAbility)
    {
        for (int i = 0; i < pendingRequestList.Count; i++)
        {
            if (pendingRequestList[i] is CleanTableRequest && pendingRequestList[i].GUID == table.GUID)
            {
                pendingRequestList.RemoveAt(i);
                break;
            }
        }

        for (int i = 0; i < workerList.Count; i++)
        {
            if (workerList[i].CurrentTask == null)
                return;

            if (workerList[i].CurrentTask is not CleanTableTask)
                continue;

            CleanTableTask cleanTableTask = workerList[i].CurrentTask as CleanTableTask;

            if (cleanTableTask.Table != table)
                continue;

            if (holdUsedObjectAbility.gameObject == workerList[i].gameObject)
                continue;

            workerList[i].CancelTask();
        }
    }
}