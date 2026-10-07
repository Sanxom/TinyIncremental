using UnityEngine;

public class Chair : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Transform customerTargetSitPoint;
    [SerializeField] private Transform customerTargetWalkPoint;
    [SerializeField] private Transform model;
    public Vector3 CustomerTargetWalkPosition => customerTargetWalkPoint.position;

    [Header("Settings")]
    [SerializeField] private float usedChairRotationAmount = 60f;
    private Customer customer;
    private bool isEmpty;
    public bool IsEmpty => isEmpty;

    private void Awake()
    {
        isEmpty = true;
    }

    public Customer Pop()
    {
        isEmpty = true;

        customer.transform.SetParent(CustomerManager.Instance.transform);

        Customer customerToReturn = customer;
        customer = null;

        return customerToReturn;
    }

    public void Push(Customer customerToServe)
    {
        customer = customerToServe;
        customer.transform.SetParent(transform);
        customer.SitDown(customerTargetSitPoint.position, transform.forward);
    }

    public void MarkAsOccupied()
    {
        isEmpty = false;
    }

    public void ChangeChairsToUsed()
    {
        model.localRotation = Quaternion.Euler(usedChairRotationAmount * Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)), usedChairRotationAmount * Mathf.Sign(UnityEngine.Random.Range(-1f, 1f)), 0f);
    }

    public void FixChairs()
    {
        model.localRotation = Quaternion.identity;
    }
}