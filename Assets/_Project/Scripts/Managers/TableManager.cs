using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    [Header("Elements")]
    private List<TableSet> tableList;

    private void Awake()
    {
        tableList = GetComponentsInChildren<TableSet>().ToList();
    }

    public bool IsAnyTableAvailable()
    {
        return GetFirstCleanEmptyTable() != null;
    }

    public void HandleCustomerServed(Customer customerToServe)
    {
        TableSet table = GetFirstCleanEmptyTable();
        if (table == null)
        {
            Debug.LogError("TableManager: No Clean Table found! This should not happen!");
            return;
        }

        table.AcceptCustomer(customerToServe, this);

        //Vector3 randomPosition = UnityEngine.Random.onUnitSphere.With(y: 0) * 5;
        //customerToServe.GoTo(randomPosition);
    }

    private TableSet GetFirstCleanEmptyTable()
    {
        for (int i = 0; i < tableList.Count; i++)
        {
            if (tableList[i].IsDirty)
                continue;
            if (tableList[i].IsFull)
                continue;

            return tableList[i];
        }

        return null;
    }
}