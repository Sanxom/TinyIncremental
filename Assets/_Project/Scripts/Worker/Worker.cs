using UnityEngine;

[RequireComponent(typeof(NavigationAbility))]
[RequireComponent(typeof(HoldObjectAbility))]
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

    [Header("Task")]
    private WorkerTask currentTask;
    private State state;

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

        state = State.Idle;
    }

    private void Update()
    {
        HandleStateMachine();

        currentTask?.Update();
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

    public void CompleteTask()
    {
        currentTask = null;
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

    public void MarkAsBusy()
    {
        state = State.PerformingTask;
    }

    public void CancelTask()
    {
        currentTask.Cancel();
        CompleteTask();
    }
}