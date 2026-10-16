using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private BaseCharacterStatsDataSO baseStats;

    [Header("Settings")]
    private float speed;
    private int capacity;
    private float revenue;

    public float Speed => speed;
    public int Capacity => capacity;
    public float Revenue => revenue;

    private void Awake()
    {
        speed = baseStats.Speed;
        capacity = baseStats.Capacity;
        revenue = baseStats.Revenue;
    }

    public void SetupStats(Vector3Int statsLevels)
    {
        speed = baseStats.Speed + CalculateAdditionalSpeed(statsLevels.x);
        capacity = baseStats.Capacity + CalculateAdditionalCapacity(statsLevels.y);
        revenue = baseStats.Revenue + CalculateAdditionalRevenue(statsLevels.z);
    }

    private float CalculateAdditionalSpeed(int level)
    {
        float increment = 0.2f;
        return level * increment;
    }

    private int CalculateAdditionalCapacity(int level)
    {
        return level;
    }

    private float CalculateAdditionalRevenue(int level)
    {
        return level * 0.2f;
    }
}