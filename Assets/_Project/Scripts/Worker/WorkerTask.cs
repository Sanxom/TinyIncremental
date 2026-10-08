using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorkerTask
{
    protected List<Subtask> subtaskList = new();
    protected Worker worker;
    protected TaskRequest request;
    protected int currentSubtaskIndex;

    public TaskRequest Request => request;

    public WorkerTask(Worker worker, TaskRequest request)
    {
        this.worker = worker;
        this.request = request;
    }

    public void Start()
    {
        currentSubtaskIndex = 0;

        subtaskList[0].Start(worker);
    }

    public void Update()
    {
        if (currentSubtaskIndex >= subtaskList.Count)
            return;

        Subtask current = subtaskList[currentSubtaskIndex];
        current.Update(worker);

        if (current.IsComplete)
        {
            currentSubtaskIndex++;

            if (currentSubtaskIndex < subtaskList.Count)
                subtaskList[currentSubtaskIndex].Start(worker);
            else
                Complete();
        }
    }

    public void Cancel()
    {
        RemoveRequestFromSender();
    }

    private void Complete()
    {
        RemoveRequestFromSender();
        worker.CompleteTask();
    }

    private void RemoveRequestFromSender()
    {
        if (request != null && request.sender != null)
            request.sender.ClearRequest(request);
    }
}

public class FillStationPlateauTask : WorkerTask
{
    public FillStationPlateauTask(Worker worker, Vector3 objectSpawningStationPosition, Vector3 dropZonePosition, TaskRequest request) : base(worker, request)
    {
        subtaskList.Add(new MoveToSubtask(objectSpawningStationPosition));
        // Wait for Plateau to be Full
        subtaskList.Add(new WaitForConditionSubtask(() => worker.IsPlateauFull && !worker.IsPlateauDirty));
        subtaskList.Add(new MoveToSubtask(dropZonePosition));
        // Wait for Plateau to be Empty
        subtaskList.Add(new WaitForConditionSubtask(() => worker.IsPlateauEmpty));
    }
}

public class ServeCustomersTask : WorkerTask
{
    public ServeCustomersTask(Worker worker, Vector3 targetPosition, TaskRequest request) : base(worker, request)
    {
        subtaskList.Add(new MoveToSubtask(targetPosition));
        // TODO: Remove this hard-coded value for a value you can set in the Inspector
        subtaskList.Add(new WaitForConditionSubtask(() => (request as ServeCustomersRequest).ObjectDropZone.ObjectCount < 3));
    }
}

public class CleanTableTask : WorkerTask
{
    public TableSet Table => (request as CleanTableRequest).Table;

    public CleanTableTask(Worker worker, TableSet table, Trash trash, TaskRequest request) : base(worker, request)
    {
        subtaskList.Add(new MoveToSubtask(table.WorkerTargetPosition));
        subtaskList.Add(new WaitForConditionSubtask(() => !table.IsDirty));
        subtaskList.Add(new MoveToSubtask(trash.WorkerTargetPosition));
        subtaskList.Add(new WaitForConditionSubtask(() => !worker.IsPlateauDirty && worker.IsPlateauEmpty));
    }
}

public class IdleTask : WorkerTask
{
    public IdleTask(Worker worker, Vector3 targetPosition, TaskRequest request) : base(worker, request)
    {
        subtaskList.Add(new MoveToSubtask(targetPosition));
        subtaskList.Add(new WaitForConditionSubtask(() => false));
    }
}

public abstract class Subtask
{
    public bool IsComplete { get; protected set; }

    public abstract void Start(Worker worker);
    public abstract void Update(Worker worker);
}

public class MoveToSubtask : Subtask
{
    Vector3 destination;

    public MoveToSubtask(Vector3 destination) => this.destination = destination;

    public override void Start(Worker worker)
    {
        worker.GoTo(destination);
    }

    public override void Update(Worker worker)
    {
        if (worker.HasReachedDestination)
            IsComplete = true;
    }
}

public class WaitForConditionSubtask : Subtask
{
    private Func<bool> condition;

    public WaitForConditionSubtask(Func<bool> condition) => this.condition = condition;

    public override void Start(Worker worker)
    {
        worker.MarkAsBusy();
    }

    public override void Update(Worker worker)
    {
        if (condition())
            IsComplete = true;
    }
}