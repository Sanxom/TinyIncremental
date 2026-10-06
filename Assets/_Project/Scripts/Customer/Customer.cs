using System;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walking,
        Drinking
    }

    [Header("Components")]
    [SerializeField] private CustomerAnimator customerAnimator;
    [SerializeField] private NavigationAbility navigationAbility;
    [SerializeField] private Plateau plateau;

    [Header("Settings")]
    private Vector3 finalFacingDirection;
    private int objectNeededCount;
    private int objectTakenCount;
    public int ObjectNeededCount => objectNeededCount;
    public int ObjectTakenCount => objectTakenCount;

    [Header("Actions")]
    private Action reachedDestinationCallback;

    private State state;

    private void Update()
    {
        HandleStateMachine();
    }

    public void Init(int objectNeededCount, Vector3 targetPosition, Vector3 finalFacingDirection)
    {
        this.objectNeededCount = objectNeededCount;
        this.finalFacingDirection = finalFacingDirection;
        GoToThen(targetPosition, FaceFinalDirection);
    }

    public bool NeedsMoreObjects()
    {
        return objectTakenCount < objectNeededCount;
    }

    public void CollectObject(SpawnableObject objectToServe)
    {
        plateau.gameObject.SetActive(true);
        plateau.Push(objectToServe);

        objectTakenCount++;
    }

    private void GoTo(Vector3 targetPosition)
    {
        bool canReachDestination = navigationAbility.TryGoTo(targetPosition);

        if (canReachDestination)
            StartWalkingState();
    }

    private void GoToThen(Vector3 targetPosition, Action callback)
    {
        reachedDestinationCallback = callback;
        GoTo(targetPosition);
    }

    private void HandleStateMachine()
    {
        switch (state)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Walking:
                HandleWalkingState();
                break;
            default:
                break;
        }
    }

    private void HandleIdleState()
    {
        if (navigationAbility.IsMoving())
        {
            StartWalkingState();
        }
    }

    private void HandleWalkingState()
    {
        if (navigationAbility.HasReachedDestination())
        {
            ReachDestination();
            return;
        }

        if (navigationAbility.IsMoving())
        {
            customerAnimator.ManageAnimations(navigationAbility.Velocity);
        }
        else
            StartIdleState();
    }

    private void ReachDestination()
    {
        StartIdleState();

        if (reachedDestinationCallback != null)
        {
            reachedDestinationCallback?.Invoke();
            reachedDestinationCallback = null;
        }
    }

    private void FaceFinalDirection()
    {
        customerAnimator.Face(finalFacingDirection);
    }

    private void StartIdleState()
    {
        state = State.Idle;
        customerAnimator.Stop();
    }

    private void StartWalkingState()
    {
        state = State.Walking;
        customerAnimator.StartWalking();
    }
}