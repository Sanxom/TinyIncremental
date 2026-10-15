using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HRDeskStation : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private HRManager hrManager;

    [Header("Settings")]
    private bool hasEntered;

    private void OnTriggerStay(Collider other)
    {
        if (hrManager.IsPanelVisible || hasEntered)
            return;
        if (!other.TryGetComponent(out PlayerController playerController))
            return;
        if (playerController.IsMoving())
            return;

        hasEntered = true;
        hrManager.Show();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController playerController))
            return;

        hasEntered = false;
    }
}