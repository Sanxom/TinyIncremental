using System;
using UnityEngine;

public class HoldUsedObjectAbility : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Plateau plateau;

    public SpawnableObject[] PopAll()
    {
        SpawnableObject[] usedObjectArray = plateau.PopAll();
        plateau.gameObject.SetActive(false);

        return usedObjectArray;
    }

    public bool CanCollectUsedObjects()
    {
        if (!plateau.gameObject.activeInHierarchy)
            return true;

        if (!plateau.IsEmpty && !plateau.IsDirty)
            return false;

        return true;
    }

    public bool HasUsedObjects()
    {
        return !plateau.IsEmpty && plateau.IsDirty;
    }

    public void CollectUsedObjects(SpawnableObject[] usedObjectArray)
    {
        for (int i = 0; i < usedObjectArray.Length; i++)
        {
            plateau.gameObject.SetActive(true);
            plateau.Push(usedObjectArray[i]);
        }
    }
}