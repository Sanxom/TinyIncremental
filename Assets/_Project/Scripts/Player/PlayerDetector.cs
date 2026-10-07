using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HoldObjectAbility))]
public class PlayerDetector : MonoBehaviour
{
    [Header("Components")]
    private HoldObjectAbility holdObjectAbility;

    private void Awake()
    {
        holdObjectAbility = GetComponent<HoldObjectAbility>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out SpawnerStation spawnerStation))
        {
            HandleObjectSpawnerStation(spawnerStation);
        }
        else if (other.TryGetComponent(out ObjectDropZone dropZone))
        {
            HandleObjectDropZone(dropZone);
        }
        else if (other.TryGetComponent(out TableSet table))
        {
            HandleTableTriggered(table);
        }
    }

    private void HandleTableTriggered(TableSet table)
    {
        if (!table.IsDirty) 
            return;
        if (!TryGetComponent(out HoldUsedObjectAbility holdUsedObjectAbility))
            return;
        if (!holdUsedObjectAbility.CanCollectUsedObjects())
            return;

        table.GetCleanedBy(holdUsedObjectAbility);
    }

    private void HandleObjectDropZone(ObjectDropZone dropZone)
    {
        holdObjectAbility.HandleInDropZone(dropZone);
    }

    private void HandleObjectSpawnerStation(SpawnerStation spawnerStation)
    {
        holdObjectAbility.HandleInObjectSpawnerStation(spawnerStation);
    }
}