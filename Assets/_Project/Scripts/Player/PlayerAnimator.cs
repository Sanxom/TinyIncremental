using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int IdleWithPlateauHash = Animator.StringToHash("IdleWithPlateau");
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int WalkWithPlateauHash = Animator.StringToHash("WalkWithPlateau");
    private static readonly int WalkHash = Animator.StringToHash("Walk");
    private static readonly int MoveSpeedHash = Animator.StringToHash("moveSpeed");

    [Header(" Elements ")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject plateau;

    public void ManageAnimations(Vector3 moveVector, float moveSpeed)
    {
        if (moveVector.magnitude > 0)
        {
            animator.SetFloat(MoveSpeedHash, moveSpeed / 1.5f);
            PlayWalkAnimation();

            animator.transform.forward = moveVector.normalized;
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
            if (plateau.activeInHierarchy)
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
            if (plateau.activeInHierarchy)
                animator.Play(IdleWithPlateauHash);
            else
                animator.Play(IdleHash);
        }
    }
}