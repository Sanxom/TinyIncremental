using System;
using UnityEngine;

[RequireComponent(typeof(NavigationAbility))]
[RequireComponent(typeof(HoldObjectAbility))]
[RequireComponent(typeof(CharacterStats))]
public class Worker : MonoBehaviour
{
    public enum State
    {
        Idle,
        PerformingTask
    }

    [Header("Components")]
    [SerializeField] private NPCAnimator animator;
    private NavigationAbility navigationAbility;
    private HoldObjectAbility holdObjectAbility;
    private WorkerManager workerManager;
    private CharacterStats stats;

    [Header("Task")]
    private WorkerTask currentTask;
    private State state;

    [Header("Settings")]
    private float revenueMultiplier;
    private int level;

    public WorkerTask CurrentTask => currentTask;
    public bool HasReachedDestination => navigationAbility.HasReachedDestination();
    public bool IsPlateauFull => holdObjectAbility.IsPlateauFull;
    public bool IsPlateauEmpty => holdObjectAbility.IsPlateauEmpty;
    public bool IsPlateauDirty => holdObjectAbility.IsPlateauDirty;
    public bool CanCancelTask => !holdObjectAbility.IsPlateauActive;

    private void Awake()
    {
        navigationAbility = GetComponent<NavigationAbility>();
        holdObjectAbility = GetComponent<HoldObjectAbility>();
        stats = GetComponent<CharacterStats>();

        state = State.Idle;
    }

    private void Update()
    {
        HandleStateMachine();

        currentTask?.Update();
    }

    public void Init(WorkerManager workerManager, int level)
    {
        this.workerManager = workerManager;
        this.level = level;

        SetupStats();
    }

    public void AssignTask(WorkerTask workerTask)
    {
        currentTask = workerTask;
        currentTask.Start();
    }

    public void GoTo(Vector3 targetPosition)
    {
        bool canReachDestination = navigationAbility.TryGoTo(targetPosition);

        if (canReachDestination)
            StartWalkingState();
    }

    public void CompleteTask()
    {
        currentTask = null;
        StartIdleState();
    }

    public void MarkAsBusy()
    {
        state = State.PerformingTask;
    }

    public void CancelTask()
    {
        currentTask.Cancel();
        CompleteTask();
    }

    public void LevelUp()
    {
        level++;
        SetupStats();
    }

    private void SetupStats()
    {
        Vector3Int levels = WorkerUtilities.GetStatLevelsFromLevel(level);
        stats.SetupStats(levels);

        navigationAbility.SetSpeed(stats.Speed);
        holdObjectAbility.SetPlateauCapacity(stats.Capacity);
        revenueMultiplier = stats.Revenue;

    }

    private void HandleStateMachine()
    {
        switch (state)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.PerformingTask:
                HandlePerformingTaskState();
                break;
            default:
                break;
        }
    }

    private void HandleIdleState()
    {
        if (navigationAbility.IsMoving())
            StartWalkingState();
    }

    private void HandlePerformingTaskState()
    {
        if (navigationAbility.HasReachedDestination())
        {
            ReachDestination();
            return;
        }

        if (navigationAbility.IsMoving())
            animator.ManageAnimations(navigationAbility.Velocity);
        else
            StartIdleState();
    }

    private void ReachDestination()
    {
        StartIdleState();
    }

    private void StartIdleState()
    {
        state = State.Idle;
        animator.Stop();
    }

    private void StartWalkingState()
    {
        MarkAsBusy();
    }
}

public static class WorkerUtilities
{
    private static readonly int numOfStats = 3;

    public static Vector3Int GetStatLevelsFromLevel(int level)
    {
        int speedLevel = Mathf.CeilToInt((float)level / numOfStats);
        int capacityLevel = Mathf.CeilToInt(Mathf.Max(0f, level - 1) / numOfStats);
        int revenueLevel = Mathf.CeilToInt(Mathf.Max(0f, level - 2) / numOfStats);

        return new Vector3Int(speedLevel, capacityLevel, revenueLevel);
    }
}