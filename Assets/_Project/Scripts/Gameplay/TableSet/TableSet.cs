using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableSet : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Plateau plateau;
    private TableManager tableManager;
    private List<Chair> chairList;

    [Header("Settings")]
    private List<Customer> customerList = new();
    private int incomingCustomers = 0;
    private bool isFull;
    private bool isDirty;
    public bool IsFull => isFull;
    public bool IsDirty => isDirty;

    [Header("Timer Settings")]
    private float timer;
    private float objectTimer;
    private int numObjectUsed;

    private void Awake()
    {
        isFull = false;
        chairList = GetComponentsInChildren<Chair>().ToList();
    }

    private void Update()
    {
        if (incomingCustomers > 0)
            return;

        if (customerList.Count > 0)
        {
            HandleCustomerLeaveTimer();
            HandleObjectTimer();
        }
    }

    public void AcceptCustomer(Customer customerToServe, TableManager tableManager)
    {
        this.tableManager = tableManager;

        Chair targetChair = GetFirstEmptyChair();
        if (targetChair == null)
        {
            Debug.LogError("TableSet: No empty Chair was found! This should not happen!");
            return;
        }

        incomingCustomers++;

        targetChair.MarkAsOccupied();

        customerToServe.GoToThen(targetChair.CustomerTargetWalkPosition, 
            () => HandleCustomerReachedChair(customerToServe, targetChair));

        CheckIfTableIsFull();
    }

    public void GetCleanedBy(HoldUsedObjectAbility holdUsedObjectAbility)
    {
        SpawnableObject[] usedObjectArray = plateau.PopAll();
        plateau.gameObject.SetActive(false);

        holdUsedObjectAbility.CollectUsedObjects(usedObjectArray);

        for (int i = 0; i < chairList.Count; i++)
            chairList[i].FixChairs();

        isDirty = false;
        isFull = false;
    }

    private Chair GetFirstEmptyChair()
    {
        for (int i = 0; i < chairList.Count; i++)
        {
            if (chairList[i].IsEmpty)
                return chairList[i];
        }

        return null;
    }

    private void HandleCustomerReachedChair(Customer customerToServe, Chair targetChair)
    {
        customerList.Add(customerToServe);

        targetChair.Push(customerToServe);

        for (int i = 0; i < customerToServe.ObjectTakenCount; i++)
        {
            SpawnableObject spawnableObject = customerToServe.Pop();

            plateau.gameObject.SetActive(true);
            plateau.Push(spawnableObject);

            timer += ConstantFields.TIME_TO_FINISH_AT_TABLE;
        }

        incomingCustomers--;
        incomingCustomers = Mathf.Max(0, incomingCustomers);
    }

    private void HandleCustomerLeaveTimer()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ForceCustomerToLeave();
            timer = 0f;
        }
    }

    private void HandleObjectTimer()
    {
        objectTimer += Time.deltaTime;

        if (objectTimer > (numObjectUsed + 1) * ConstantFields.TIME_TO_FINISH_AT_TABLE)
        {
            HideNextObject();
        }
    }

    private void HideNextObject()
    {
        numObjectUsed++;
        plateau.HideObject();
    }

    private void ForceCustomerToLeave()
    {
        customerList.Clear();

        objectTimer = 0f;
        numObjectUsed = 0;

        for (int i = 0; i < chairList.Count; i++)
        {
            if (chairList[i].IsEmpty)
                continue;

            Customer customer = chairList[i].Pop();
            CustomerManager.Instance.HandleForcingCustomerToLeave(customer);

            chairList[i].ChangeChairsToUsed();
        }

        isDirty = true;

        plateau.MarkAsDirty();

        isFull = false;
    }

    private void CheckIfTableIsFull()
    {
        for (int i = 0; i < chairList.Count; i++)
        {
            if (chairList[i].IsEmpty)
            {
                isFull = false;
                return;
            }
        }

        isFull = true;
    }
}