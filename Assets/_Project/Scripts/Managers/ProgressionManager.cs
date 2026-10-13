using System.Collections;
using System.Collections.Generic;
using Tabsil.Sijil;
using Unity.Cinemachine;
using UnityEngine;

public class ProgressionManager : MonoBehaviour, IWantToBeSaved
{
    private static WaitForSeconds delayTime = new(1f);
    [Header("Elements")]
    [SerializeField] private CinemachineCamera progressionCamera;
    [SerializeField] private GameObject progressionCanvas;

    [Header("Data")]
    [SerializeField] private ProgressionStep[] progressionStepArray;
    private int progressionIndex;

    private const string PROGRESSION_INDEX_KEY = "PROGRESSION_INDEX";

    private void Awake()
    {
        progressionCanvas.SetActive(false);
        LockedElement.OnUnlocked += OnLockedElementUnlocked;
    }

    private void OnDestroy()
    {
        LockedElement.OnUnlocked -= OnLockedElementUnlocked;
    }

    public void Load()
    {
        if (Sijil.TryLoad(this, PROGRESSION_INDEX_KEY, out object _progressionIndex))
            progressionIndex = (int)_progressionIndex;

        if (progressionIndex >= progressionStepArray.Length)
            return;

        for (int i = progressionIndex; i < progressionStepArray.Length; i++)
            progressionStepArray[i].Hide();

        ProgressionStep step = progressionStepArray[progressionIndex];
        step.Display();
    }

    public void Save()
    {
        Sijil.Save(this, PROGRESSION_INDEX_KEY, progressionIndex);
    }

    private IEnumerator StartStepCoroutine(ProgressionStep step)
    {
        CameraTarget target = new();

        for (int i = 0; i < step.lockedElementList.Count; i++)
        {
            target.TrackingTarget = step.lockedElementList[i].transform;
            progressionCamera.Target = target;
            progressionCamera.gameObject.SetActive(true);

            yield return delayTime;
        }

        progressionCamera.gameObject.SetActive(false);
        progressionCanvas.SetActive(false);
    }

    private void OnLockedElementUnlocked(LockedElement element)
    {
        if (!progressionStepArray[progressionIndex].Contains(element))
        {
            Debug.LogError("Current Progression Step does not contain this locked element!");
            return;
        }

        progressionStepArray[progressionIndex].Unlock(element);

        if (!progressionStepArray[progressionIndex].IsComplete())
            return;

        progressionIndex++;
        Save();

        if (progressionIndex >= progressionStepArray.Length)
            return;

        StartNextStep();
    }

    private void StartNextStep()
    {
        progressionCanvas.SetActive(true);
        ProgressionStep step = progressionStepArray[progressionIndex];
        step.Display();
        StartCoroutine(StartStepCoroutine(step));
    }
}

[System.Serializable]
public struct ProgressionStep
{
    public string name;
    public List<LockedElement> lockedElementList;

    private List<LockedElement> unlockedElementList;

    public bool Contains(LockedElement element) => lockedElementList.Contains(element);
    public bool IsComplete() => unlockedElementList.Count >= lockedElementList.Count;

    public void Unlock(LockedElement element)
    {
        unlockedElementList ??= new List<LockedElement>();

        unlockedElementList.Add(element);
    }

    public void Hide()
    {
        for (int i = 0; i < lockedElementList.Count; i++)
            lockedElementList[i].gameObject.SetActive(false);
    }

    public void Display()
    {
        for (int i = 0; i < lockedElementList.Count; i++)
            lockedElementList[i].gameObject.SetActive(true);
    }
}