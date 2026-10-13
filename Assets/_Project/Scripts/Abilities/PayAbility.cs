using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PayAbility : MonoBehaviour
{
    [Header("Components")]
    private CharacterController characterController;

    [Header("Elements")]
    [SerializeField] private ObjectPooler cashObjectPool;
    [SerializeField] private GameObject cashPrefab;

    public bool IsMoving => characterController.velocity.magnitude > 0.1f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Pay(LockedElement lockedElement, int cashPerFrame)
    {
        int toPay = Mathf.Min(cashPerFrame, CurrencyManager.Instance.Currency);

        if (toPay <= 0) return;

        CurrencyManager.Instance.AddCurrency(-toPay);
        lockedElement.CollectCashToPay(toPay);

        AnimateCashToTarget(lockedElement.transform);
    }

    private void AnimateCashToTarget(Transform target)
    {
        GameObject cashObject = cashObjectPool.GetPooledObject();
        cashObject.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        ArcAnimator.Instance.Animate(cashObject.transform, target, 0.2f, 0f, 2f, () => cashObjectPool.ReturnObjectToPool(cashObject));
    }
}