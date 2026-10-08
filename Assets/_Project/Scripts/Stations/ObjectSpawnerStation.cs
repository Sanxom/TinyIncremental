using System;
using UnityEngine;

public class ObjectSpawnerStation : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpawnableObject spawnableObjectPrefab;
    [SerializeField] private Plateau plateau;
    [SerializeField] private Transform workerTargetPoint;

    [Header("Settings")]
    [SerializeField] private float spawnDelay;
    private float spawnTimer;

    public Type ObjectType => spawnableObjectPrefab.GetType();
    public Vector3 WorkerTargetPosition => workerTargetPoint.position;

    private void Update()
    {
        HandleSpawnTimer();
    }

    public SpawnableObject Pop()
    {
        SpawnableObject spawnableObject = plateau.Pop();

        if (spawnableObject == null)
            return null;

        return spawnableObject;
    }

    private void HandleSpawnTimer()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnDelay)
        {
            TrySpawnObject();
            spawnTimer = 0f;
        }
    }

    private void TrySpawnObject()
    {
        if (plateau.IsFull) return;

        SpawnFood();
    }

    private void SpawnFood()
    {
        SpawnableObject objectInstance = Instantiate(spawnableObjectPrefab, transform);
        plateau.Push(objectInstance);
    }
}