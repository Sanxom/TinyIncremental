using System;
using UnityEngine;

public class ObjectDropZone : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Plateau plateau;

    [Header("Settings")]
    [SerializeField] private Transform workerTargetPoint;
    [SerializeField] private SpawnableObject acceptedObject;

    public SpawnableObject AcceptedObject => acceptedObject;
    public Transform WorkerTargetPoint => workerTargetPoint;
    public Vector3 WorkerTargetPosition => workerTargetPoint.position;
    public bool IsFull => plateau.IsFull;
    public int ObjectCount => plateau.GetObjectCountInPlateau();

    public ObjectPosition GetFirstObjectPosition() => plateau.GetFirstObjectPosition();

    public SpawnableObject Pop() => plateau.Pop();

    public bool CanAcceptObject(SpawnableObject spawnableObject) => spawnableObject.GetType() == acceptedObject.GetType();

    public void Push(SpawnableObject spawnableObject) => plateau.Push(spawnableObject);

    public void SetAcceptedObject(SpawnableObject acceptedObject) => this.acceptedObject = acceptedObject;
}