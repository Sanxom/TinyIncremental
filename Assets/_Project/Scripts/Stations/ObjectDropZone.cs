using System;
using UnityEngine;

public class ObjectDropZone : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Plateau plateau;

    public bool IsFull => plateau.IsFull;

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