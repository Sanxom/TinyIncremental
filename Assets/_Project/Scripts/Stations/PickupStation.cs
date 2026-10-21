using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupStation : ObjectServingStation
{
    [Header("Elements")]
    [SerializeField] private Transform customerExitPoint;

    protected override void ServeObject(CharacterStats characterStats)
    {
        ServeCustomer(characterStats);
        servingTimer = 0f;
    }

    protected override void DequeueCustomer(Customer customer)
    {
        objectServingCustomerManager.Dequeue();

        HandleCustomerServed(customer);
    }

    private void HandleCustomerServed(Customer customer)
    {
        customer.GoToThen(customerExitPoint.position, () => CustomerManager.Instance.CustomerObjectPool.ReturnObjectToPool(customer.gameObject));
    }
}