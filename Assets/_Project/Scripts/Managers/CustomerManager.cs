using System;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [Header("Elements")]
    [SerializeField] private ObjectPooler customerObjectPool;
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
        GameObject customerObject = customerObjectPool.GetPooledObject();
        Customer customer = customerObject.GetComponent<Customer>();
        customerObject.transform.SetParent(transform);
        customerObject.transform.SetLocalPositionAndRotation(spawnPosition, Quaternion.identity);
        customerObject.SetActive(true);
        return customer;
    }

    public void HandleForcingCustomerToLeave(Customer customer)
    {
        customer.GetUpAndGo(customerExitPoint.position, () => HandleCustomerReachedExitPoint(customer));
    }

    private void HandleCustomerReachedExitPoint(Customer customer)
    {
        customerObjectPool.ReturnObjectToPool(customer.gameObject);
    }
}