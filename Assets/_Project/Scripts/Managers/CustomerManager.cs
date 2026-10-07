using System;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [Header("Elements")]
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private Transform customerExitPoint;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Customer Pop(Vector3 spawnPosition)
    {
        // TODO: return an ObjectPooled Customer instead
        return Instantiate(customerPrefab, spawnPosition, Quaternion.identity, transform);
    }

    public void HandleForcingCustomerToLeave(Customer customer)
    {
        customer.GetUpAndGo(customerExitPoint.position, () => HandleCustomerReachedExitPoint(customer));
    }

    private void HandleCustomerReachedExitPoint(Customer customer)
    {
        // TODO: Return Customer to ObjectPool here instead
        Destroy(customer.gameObject);
    }
}