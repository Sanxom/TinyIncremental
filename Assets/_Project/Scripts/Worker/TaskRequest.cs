using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TaskRequest
{
    public TaskRequester sender;

    protected string guid;
    protected int priority;

    public string GUID => guid;
    public int Priority => priority;

    public TaskRequest(TaskRequester sender) => this.sender = sender;

    public TaskRequest() { }
}

public class FillStationPlateauRequest : TaskRequest
{
    private SpawnableObject spawnObject;
    private Vector3 dropZonePosition;

    public SpawnableObject SpawnObject => spawnObject;
    public Vector3 DropZonePosition => dropZonePosition;

    public FillStationPlateauRequest(string guid, SpawnableObject spawnableObject, Vector3 dropZonePosition)
    {
        this.guid = guid;
        spawnObject = spawnableObject;
        this.dropZonePosition = dropZonePosition;
        priority = 70;
    }
}

public class ServeCustomersRequest : TaskRequest
{
    public Vector3 WorkerTargetPosition { get; private set; }
    public ObjectDropZone ObjectDropZone { get; private set; }

    public ServeCustomersRequest(string guid, Vector3 workerTargetPosition, ObjectDropZone dropZone)
    {
        this.guid = guid;
        WorkerTargetPosition = workerTargetPosition;
        ObjectDropZone = dropZone;

        priority = 40;
    }
}

public class CleanTableRequest : TaskRequest
{
    public TableSet Table { get; private set; }

    public CleanTableRequest(string guid, TableSet table)
    {
        this.guid = guid;
        Table = table;

        priority = 50;
    }
}

public class IdleRequest : TaskRequest
{
    public Vector3 TargetPosition { get; private set; }

    public IdleRequest(string guid, Vector3 targetPosition)
    {
        this.guid = guid;
        TargetPosition = targetPosition;

        priority = -1;
    }
}