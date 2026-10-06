using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerStation : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpawnableObject spawnableObjectPrefab;
    [SerializeField] private Plateau plateau;

    [Header("Settings")]
    [SerializeField] private float spawnDelay;
    private float spawnTimer;

    private void Update()
    {
        HandleSpawnTimer();
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