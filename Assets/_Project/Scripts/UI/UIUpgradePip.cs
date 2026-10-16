using DG.Tweening;
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
    [SerializeField] private Color blinkColor;

    public void Activate()
    {
        image.DOKill();
        image.color = activeColor;
    }

    public void Blink()
    {
        image.DOKill();
        image.color = Color.white;
        image.DOColor(blinkColor, 1f).SetLoops(-1, LoopType.Yoyo);
    }
}