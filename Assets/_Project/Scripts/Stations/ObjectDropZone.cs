using UnityEngine;

public class ObjectDropZone : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Plateau plateau;

    [Header("Settings")]
    [SerializeField] private Transform workerTargetPoint;

    public Vector3 WorkerTargetPosition => workerTargetPoint.position;
    public bool IsFull => plateau.IsFull;
    public int ObjectCount => plateau.GetObjectCountInPlateau();

    public ObjectPosition GetFirstObjectPosition()
    {
        return plateau.GetFirstObjectPosition();
    }

    public SpawnableObject Pop()
    {
        return plateau.Pop();
    }

    public void Push(SpawnableObject spawnableObject)
    {
        plateau.Push(spawnableObject);
    }
}