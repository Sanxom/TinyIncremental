using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GuidGenerator))]
[RequireComponent(typeof(TaskRequester))]
public class WorkerIdlePosition : MonoBehaviour
{
    [Header("Components")]
    private GuidGenerator guidGenerator;
    private TaskRequester taskRequester;

    [Header("Settings")]
    [SerializeField] private float taskRequestTimerDelay = 2f;
    private float taskRequestTimer;

    private void Awake()
    {
        guidGenerator = GetComponent<GuidGenerator>();
        taskRequester = GetComponent<TaskRequester>();
    }

    private void Update()
    {
        taskRequestTimer += Time.deltaTime;

        if (taskRequestTimer >= taskRequestTimerDelay)
        {
            taskRequestTimer = 0f;
            taskRequester.CreateTaskRequest(new IdleRequest(guidGenerator.GUID, transform.position));
        }
    }
}