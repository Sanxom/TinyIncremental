using UnityEngine;
using Tabsil.Sijil;

[RequireComponent(typeof(PlayerAnimator))]
public abstract class PlayerController : MonoBehaviour, IWantToBeSaved
{
    [Header(" Elements ")]
    protected PlayerAnimator playerAnimator;

    [Header(" Settings ")]
    [SerializeField] protected float moveSpeed;
    private Vector3 lastPosition;

    [Header(" Data ")]
    private const string lastPositionKey = "LAST_POSITION";


    protected virtual void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    protected virtual void Update()
    {
        UpdateMovement();
        TrySaveLastPosition();
    }

    private void LateUpdate()
    {
        transform.position = transform.position.With(y: 0);   
    }

    protected abstract void UpdateMovement();

    private void TrySaveLastPosition()
    {
        if(Vector3.Distance(transform.position, lastPosition) > 4)
        {
            lastPosition = transform.position;
            Save();
        }
    }

    public abstract bool IsMoving();

    public virtual void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void Load()
    {
        if (Sijil.TryLoad(this, lastPositionKey, out object _lastPosition))
            transform.position = (Vector3)_lastPosition;
    }

    public void Save()
    {
        Sijil.Save(this, lastPositionKey, transform.position);
    }
}
