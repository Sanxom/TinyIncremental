using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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