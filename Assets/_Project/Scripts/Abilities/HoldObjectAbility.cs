using UnityEngine;

public class HoldObjectAbility : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Plateau plateau;

    [Header("Delay Timers")]
    [SerializeField] private float canGrabObjectDelay = 0.1f;
    [SerializeField] private float canDropObjectDelay = 0.1f;
    private float grabObjectTimer;
    private float dropObjectTimer;

    public bool IsPlateauFull => plateau.IsFull;
    public bool IsPlateauEmpty => plateau.IsEmpty;
    public bool IsPlateauDirty => plateau.IsDirty;
    public bool IsPlateauActive => plateau.gameObject.activeInHierarchy;

    private void Awake()
    {
        grabObjectTimer = canGrabObjectDelay;
    }

    public void HandleInObjectSpawnerStation(ObjectSpawnerStation spawnerStation)
    {
        // TODO: Mason Added || plateau.IsDirty
        if (plateau.IsFull || plateau.IsDirty) return;

        if (grabObjectTimer < canGrabObjectDelay)
        {
            grabObjectTimer += Time.deltaTime;
            return;
        }

        SpawnableObject objectToGrab = spawnerStation.Pop();

        if (objectToGrab == null) return;

        plateau.gameObject.SetActive(true);
        plateau.Push(objectToGrab);

        grabObjectTimer = 0f;
    }

    public void HandleInDropZone(ObjectDropZone dropZone)
    {
        if (!plateau.gameObject.activeInHierarchy) return;
        if (plateau.IsDirty) return;
        if (dropZone.IsFull) return;

        if (dropObjectTimer < canDropObjectDelay)
        {
            dropObjectTimer += Time.deltaTime;
            return;
        }

        SpawnableObject spawnableObject = plateau.Pop();

        dropZone.Push(spawnableObject);

        if (plateau.IsEmpty)
            plateau.gameObject.SetActive(false);

        dropObjectTimer = 0f;
    }
}