using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradePip : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Image image;

    [Header("Settings")]
    [SerializeField] private Color activeColor;

    public void Activate()
    {
        image.color = activeColor;
    }
}