using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierStation : ObjectServingStation
{
    [Header("Elements")]
    [SerializeField] private TableManager tableManager;

    protected override void DequeueCustomer(Customer customer)
    {
        if (!tableManager.IsAnyTableAvailable())
            return;
        objectServingCustomerManager.Dequeue();

        tableManager.HandleCustomerServed(customer);
    }

    protected override void ServeObject(CharacterStats characterStats)
    {
        if (!tableManager.IsAnyTableAvailable())
            return;

        ServeCustomer(characterStats);
        servingTimer = 0f;
    }
}