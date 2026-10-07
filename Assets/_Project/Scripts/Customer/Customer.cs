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

    public SpawnableObject Pop()
    {
        SpawnableObject spawnableObject = plateau.Pop();
        if (spawnableObject == null)
            return null;

        if (plateau.IsEmpty)
            plateau.gameObject.SetActive(false);

        return spawnableObject;
    }

    public bool NeedsMoreObjects()
    {
        return objectTakenCount < objectNeededCount;
    }

    public void CollectObject(SpawnableObject objectToServe)
    {
        plateau.gameObject.SetActive(true);
        plateau.Push(objectToServe);

        // customerAnimator.ManageAnimations(Vector3.zero);

        objectTakenCount++;
    }

    public void GoToThen(Vector3 targetPosition, Action callback)
    {
        reachedDestinationCallback = callback;
        GoTo(targetPosition);
    }

    public void GoTo(Vector3 targetPosition)
    {
        bool canReachDestination = navigationAbility.TryGoTo(targetPosition);

        if (canReachDestination)
            StartWalkingState();
    }

    public void SitDown(Vector3 targetPosition, Vector3 facingDirection)
    {
        DisableNavigation();

        transform.position = targetPosition.With(y:0);
        StartDrinkingState(facingDirection);
    }

    private void StartDrinkingState(Vector3 facingDirection)
    {
        state = State.Drinking;
        customerAnimator.PlaySitDownAnimation(facingDirection);
    }

    private void DisableNavigation()
    {
        navigationAbility.Disable();
    }

    private void EnableNavigation()
    {
        navigationAbility.Enable();
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

    public void GetUpAndGo(Vector3 position, Action reachedDestinationCallback)
    {
        EnableNavigation();
        GoToThen(position, reachedDestinationCallback);
    }
}