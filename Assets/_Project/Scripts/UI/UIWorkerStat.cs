using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorkerStat : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private UIUpgradePip upgradePipPrefab;
    [SerializeField] private Transform upgradePipParent;
    private List<UIUpgradePip> pipList = new();

    [Header("Settings")]
    private int blinkingPipIndex;

    public void Init(int maxPipCount, int activePipCount)
    {
        for (int i = 0; i < maxPipCount; i++)
        {
            UIUpgradePip pipInstance = Instantiate(upgradePipPrefab, upgradePipParent);
            pipList.Add(pipInstance);

            if (activePipCount > i)
                pipInstance.Activate();
        }

        blinkingPipIndex = activePipCount;
    }

    public void Increment()
    {
        pipList[blinkingPipIndex].Activate();
        blinkingPipIndex++;
    }

    public void Blink()
    {
        if (blinkingPipIndex > pipList.Count - 1)
            return;

        pipList[blinkingPipIndex].Blink();
    }
}