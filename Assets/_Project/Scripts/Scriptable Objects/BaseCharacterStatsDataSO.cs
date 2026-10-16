using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseCharacterStatsDataSO", menuName = "Scriptable Objects/BaseCharacterStatsDataSO")]
public class BaseCharacterStatsDataSO : ScriptableObject
{
    [Header("Stats")]
    [SerializeField, Range(1, 4)] private float speed;
    [SerializeField, Range(7, 14)] private int capacity;
    [SerializeField, Range(1, 2)] private float revenue;

    public float Speed => speed;
    public int Capacity => capacity;
    public float Revenue => revenue;
}