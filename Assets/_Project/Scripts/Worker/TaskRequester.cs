using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskRequester : MonoBehaviour
{
    [SerializeReference] private List<TaskRequest> requestList = new();

    public void CreateTaskRequest(TaskRequest request)
    {
        request.sender = this;

        foreach (TaskRequest r in requestList)
            if (r.GUID == request.GUID && r.GetType() == request.GetType())
                return;

        requestList.Add(request);

        WorkerManager.Instance.RegisterRequest(request);
    }

    public void ClearRequest(TaskRequest request)
    {
        for (int i = requestList.Count - 1; i >= 0; i--)
        {
            if (requestList[i].GUID == request.GUID && requestList[i].GetType() == request.GetType())
            {
                requestList.RemoveAt(i);
                break;
            }
        }
    }
}