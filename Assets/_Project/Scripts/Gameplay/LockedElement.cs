using UnityEngine;
using Tabsil.Sijil;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

[RequireComponent(typeof(GuidGenerator))]
public class LockedElement : MonoBehaviour, IWantToBeSaved
{
    [Header("Components")]
    private GuidGenerator guidGenerator;

    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Transform anim;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject unlockedElementsGameObject;

    [Header("Settings")]
    [SerializeField] private int initialPrice;
    private int currentPrice;
    private bool isLoaded;

    [Header("Events")]
    public static Action<LockedElement> OnUnlocked;
    [SerializeField] private UnityEvent unlockedCallback;

    private const string CURRENT_PRICE_KEY = "LOCKED_ELEMENT_CURRENT_PRICE";

    private void Awake()
    {
        guidGenerator = GetComponent<GuidGenerator>();

        currentPrice = initialPrice;
        priceText.text = $"{currentPrice}";

        if (unlockedElementsGameObject != null)
            unlockedElementsGameObject.SetActive(false);
    }

    private void Start()
    {
        if (!isLoaded)
            Load();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PayAbility _))
            return;

        ScaleUp();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out PayAbility payAbility))
            return;
        if (payAbility.IsMoving)
            return;

        float wholeAnimationDuration = 1f;
        float animationDuration = wholeAnimationDuration * ((float)currentPrice / initialPrice);
        int frameDuration = Mathf.CeilToInt(animationDuration * Application.targetFrameRate);
        frameDuration = Mathf.Max(frameDuration, 1);

        int cashPerFrame = Mathf.CeilToInt((float)currentPrice / frameDuration);

        payAbility.Pay(this, cashPerFrame);

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out PayAbility _))
            return;

        ScaleDown();
    }

    public void CollectCashToPay(int amount)
    {
        currentPrice -= amount;
        UpdateVisuals();

        if (currentPrice <= 0)
            Unlock();

        Save();
    }

    public void Load()
    {
        isLoaded = true;

        string guid = guidGenerator.GUID;
        if (Sijil.TryLoad(this, guid, out _))
            Unlock(true);
        else
        {
            if (Sijil.TryLoad(this, $"{guid}{CURRENT_PRICE_KEY}", out object _currentPrice))
            {
                currentPrice = (int)_currentPrice;
                UpdateVisuals();
            }
        }
    }

    public void Save()
    {
        string guid = guidGenerator.GUID;

        if (currentPrice > 0)
            Sijil.Save(this, $"{guid}{CURRENT_PRICE_KEY}", currentPrice);
        else
            Sijil.Save(this, guid, currentPrice);
    }

    private void UpdateVisuals()
    {
        fillImage.fillAmount = 1 - ((float)currentPrice / initialPrice);

        priceText.text = $"{currentPrice}";
    }

    private void Unlock(bool isUnlocked = false)
    {
        currentPrice = 0;
        anim.gameObject.SetActive(false);

        if (unlockedElementsGameObject != null)
        {
            string oldName = gameObject.name;
            gameObject.name = oldName.Replace("Locked", "Unlocked").Trim(); ;
            unlockedElementsGameObject.SetActive(true);
        }

        // Animate unlocked elements
        // Play particles

        unlockedCallback?.Invoke();

        if (!isUnlocked)
        {
            OnUnlocked?.Invoke(this);
            Save();
        }
    }

    private void ScaleUp()
    {
        anim.transform.DOKill();
        anim.transform
            .DOScale(Vector3.one * 1.2f, 0.2f)
            .SetEase(Ease.InOutCubic);
    }

    private void ScaleDown()
    {
        anim.transform.DOKill();
        anim.transform
            .DOScale(Vector3.one, 0.2f)
            .SetEase(Ease.InOutCubic);
    }
}