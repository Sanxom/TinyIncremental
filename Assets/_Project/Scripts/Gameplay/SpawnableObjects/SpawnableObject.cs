using UnityEngine;

public abstract class SpawnableObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cleanYOffsetOnPlateau;

    public float CleanYOffsetOnPlateau => cleanYOffsetOnPlateau;
}