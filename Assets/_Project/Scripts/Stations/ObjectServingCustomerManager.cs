using System.Collections.Generic;
using UnityEngine;

public class ObjectServingCustomerManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform customerSpawnPoint;
    [SerializeField] private Transform queueStartPoint;

    [Header("Settings")]
    [SerializeField] private Vector3 queueSpacing;
    [SerializeField] private Vector2Int minMaxCustomerObjectCount;
    [SerializeField] private int maxCustomersAllowed;
    private int customerNum = 0;

    private Queue<Customer> customerQueue = new();
    //private Dictionary<int, Customer> customerDictionary = new();

    private void Start()
    {
        StartSpawningCustomers();
    }

    public Customer PeekFirstCustomer()
    {
        return customerQueue.Peek();
    }

    public bool IsCustomerReadyToTakeObject()
    {
        if (customerQueue.Count <= 0)
            return false;

        Customer customer = customerQueue.Peek();

        float distance = Vector3.Distance(customer.transform.position.With(y:0), queueStartPoint.position);

        return distance <= 0.1f;
    }

    public void Dequeue()
    {
        customerQueue.Dequeue();

        for (int i = 0; i < customerQueue.Count; i++)
            customerQueue.ToArray()[i].GoTo(GetTargetCustomerPosition(i));
    }

    private Vector3 GetTargetCustomerPosition(int index)
    {
        return queueStartPoint.position + queueSpacing * index;
    }

    private Vector3 GetLastCustomerPosition()
    {
        return queueStartPoint.position + queueSpacing * (customerQueue.Count - 1);
    }

    private void StartSpawningCustomers()
    {
        InvokeRepeating(nameof(SpawnNewCustomer), 1f, 1f);
    }

    private void SpawnNewCustomer()
    {
        #region Dictionary Code Maybe?(Needs a bit of work)
        //if (customerDictionary.Count >= maxCustomersAllowed) return;

        //customerNum = UnityEngine.Random.Range(1, maxCustomersAllowed + 1);

        //while (!customerDictionary.ContainsKey(customerNum))
        //    customerNum = UnityEngine.Random.Range(1, maxCustomersAllowed + 1);

        //Customer newCustomer = CustomerManager.Instance.Pop(customerSpawnPosition);
        //newCustomer.name = $"Customer {customerNum}";

        //customerDictionary.Add(customerNum, newCustomer);
        #endregion

        if (customerQueue.Count >= maxCustomersAllowed) return;

        customerNum++;

        Customer newCustomer = CustomerManager.Instance.Pop(customerSpawnPoint.position);
        newCustomer.gameObject.name = $"Customer {customerNum}";

        customerQueue.Enqueue(newCustomer);

        Vector3 targetPosition = GetLastCustomerPosition();

        int objectCount = Random.Range(minMaxCustomerObjectCount.x, minMaxCustomerObjectCount.y + 1);

        newCustomer.Init(objectCount, targetPosition, -queueSpacing.normalized);
    }
}