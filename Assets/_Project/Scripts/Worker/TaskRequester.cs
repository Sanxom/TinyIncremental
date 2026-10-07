using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskRequester : MonoBehaviour
{
    private List<TaskRequest> requestList = new();

    public void CreateTaskRequest(TaskRequest request)
    {
        request.sender = this;

        print($"TaskRequester: Received a request: {request.GetType()}");

        foreach (TaskRequest r in requestList)
            if (r.GUID == request.GUID && r.GetType() == request.GetType())
                return;

        requestList.Add(request);
        print($"TaskRequester: Registered the request with {request.GetType()}");

        WorkerManager.Instance.RegisterRequest(request);
    }
}