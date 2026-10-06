using System;
using UnityEngine;

public class CustomerAnimator : MonoBehaviour
{
    private static readonly int IdleWithPlateauHash = Animator.StringToHash("IdleWithPlateau");
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int WalkWithPlateauHash = Animator.StringToHash("WalkWithPlateau");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int MoveSpeedHash = Animator.StringToHash("moveSpeed");

    [Header("Elements")]
    [SerializeField] private Animator animator;
    [SerializeField] private Plateau plateau;

    [Header("Settings")]
    [SerializeField] private float targetFrameRate = 60f;
    [SerializeField] private float rotationSpeed = 0.2f;
    private Vector3 lastVelocity;
    private bool isSitting;

    private void Update()
    {
        if (isSitting) return;

        HandleAnimations();
    }

    public void ManageAnimations(Vector3 velocity)
    {
        lastVelocity = velocity;
    }

    public void Face(Vector3 finalFacingDirection)
    {
        animator.transform.forward = finalFacingDirection;
    }

    private void HandleAnimations()
    {
        if (lastVelocity.magnitude > 0)
        {
            animator.SetFloat(MoveSpeedHash, lastVelocity.magnitude / 1.5f);
            PlayWalkAnimation();

            animator.transform.forward = Vector3.Lerp(animator.transform.forward, lastVelocity.normalized, Time.deltaTime * targetFrameRate * rotationSpeed);
        }
        else
        {
            PlayIdleAnimation();
        }
    }

    private void PlayWalkAnimation()
    {
        if (plateau == null)
            animator.Play(WalkHash);
        else
        {
            if (plateau.gameObject.activeInHierarchy)
                animator.Play(WalkWithPlateauHash);
            else
                animator.Play(WalkHash);
        }
    }

    private void PlayIdleAnimation()
    {
        if (plateau == null)
            animator.Play(IdleHash);
        else
        {
            if (plateau.gameObject.activeInHierarchy)
                animator.Play(IdleWithPlateauHash);
            else
                animator.Play(IdleHash);
        }
    }

    public void StartWalking()
    {
        isSitting = false;
    }

    public void Stop()
    {
        lastVelocity = Vector3.zero;
    }
}