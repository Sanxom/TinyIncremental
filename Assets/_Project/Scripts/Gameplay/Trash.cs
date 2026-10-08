using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform workerTargetPoint;

    public Vector3 WorkerTargetPosition => workerTargetPoint.position;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out HoldUsedObjectAbility holdUsedObjectAbility))
            return;
        if (!holdUsedObjectAbility.HasUsedObjects())
            return;

        SpawnableObject[] usedObjectArray = holdUsedObjectAbility.PopAll();

        // TODO: Return usedObjects to their ObjectPool instead
        for (int i = usedObjectArray.Length - 1; i >= 0; i--)
            Destroy(usedObjectArray[i].gameObject);
    }
}