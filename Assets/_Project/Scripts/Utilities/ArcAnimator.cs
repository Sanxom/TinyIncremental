using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcAnimator : MonoBehaviour
{
    public static ArcAnimator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Animate(Transform objectTransform, Transform targetTransform, float duration, float delay, float arcHeight, Action OnCompleteCallback)
    {
        AnimateInternal(objectTransform, targetTransform, duration, delay, arcHeight, OnCompleteCallback);
    }

    private void AnimateInternal(Transform objectTransform, Transform targetTransform, float duration, float delay, float arcHeight, Action onCompleteCallback)
    {
        StartCoroutine(MoveAlongArcCoroutine(objectTransform, targetTransform, duration, delay, arcHeight, onCompleteCallback));
    }

    private IEnumerator MoveAlongArcCoroutine(Transform objectTransform, Transform targetTransform, float duration, float delay, float arcHeight, Action onCompleteCallback)
    {
        yield return new WaitForSeconds(delay);

        if (objectTransform == null)
        {
            onCompleteCallback?.Invoke();
            yield break;
        }

        float timer = 0f;
        Vector3 startPosition = objectTransform.position;

        while (timer < duration)
        {
            float percent = timer / duration;

            Vector3 pos = Vector3.Lerp(startPosition, targetTransform.position, percent);

            pos.y += Mathf.Sin(percent * Mathf.PI) * arcHeight;
            objectTransform.position = pos;

            timer += Time.deltaTime;
            yield return null;
        }

        objectTransform.position = targetTransform.position;
        onCompleteCallback?.Invoke();
    }
}