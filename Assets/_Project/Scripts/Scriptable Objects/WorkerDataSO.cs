using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkerDataSO", menuName = "Scriptable Objects/WorkerDataSO")]
public class WorkerDataSO : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private new string name;
    [SerializeField] private int unlockPrice;
    [SerializeField] private int initialLevel;
    [SerializeField] private Sprite profilePicture;

    [Header("Stats")]
    [SerializeField, Range(2, 8)] private int maxSpeed;
    [SerializeField, Range(2, 8)] private int maxCapacity;
    [SerializeField, Range(2, 8)] private int maxRevenue;

    [Header("Gameplay")]
    [SerializeField] private Worker workerPrefab;

    #region Properties
    public string Name => name;
    public int UnlockPrice => unlockPrice;
    public int InitialLevel => initialLevel;
    public int MaxSpeed => maxSpeed;
    public int MaxCapacity => maxCapacity;
    public int MaxRevenue => maxRevenue;
    public Sprite ProfilePicture => profilePicture;
    public Worker WorkerPrefab => workerPrefab;
    #endregion
}