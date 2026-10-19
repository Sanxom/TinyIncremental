using UnityEngine;
using Tabsil.Sijil;
using TMPro;
using System;

public class CurrencyManager : MonoBehaviour, IWantToBeSaved
{
    public static CurrencyManager Instance { get; private set; }

    [Header(" Settings ")]
    private bool shouldSave;

    public int Currency { get; private set; }

    private const string currencyKey = "currency";

    [Header(" Actions ")]
    public static Action OnUpdated;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        InvokeRepeating(nameof(SaveIfNeeded), 0, 1);
    }

    public void Load()
    {
        if (Sijil.TryLoad(this, currencyKey, out object _currency))
            Currency = (int)_currency;
        else
            Currency = 525;

        UpdateTexts();
    }

    [NaughtyAttributes.Button]
    public void Add500Currency()
    {
        AddCurrency(500);    
    }

    [NaughtyAttributes.Button]
    public void Subtract10Currency()
    {
        RemoveCurrency(10);
    }

    public void AddCurrency(int amount)
    {
        Currency += amount;
        SaveAndUpdateVisuals();
    }

    public void RemoveCurrency(int amount)
    {
        Currency -= amount;
        SaveAndUpdateVisuals();
    }

    private void SaveAndUpdateVisuals()
    {
        shouldSave = true;
        UpdateTexts();
        OnUpdated?.Invoke();
    }

    public void UseCurrency(int amount) => RemoveCurrency(amount);

    public bool HasEnoughCurrency(int amount) => Currency >= amount;
    public bool HasEnoughPremiumCurrency(int amount) => Currency >= amount;

    public bool TryPurchase(int price)
    {
        if(HasEnoughCurrency(price))
        {
            RemoveCurrency(price);
            return true;
        }

        return false;
    }

    private void UpdateTexts()
    {
        // We can cache these if we want more performance
        CurrencyText[] currencyTexts = 
            FindObjectsByType<CurrencyText>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (CurrencyText ct in currencyTexts)
            ct.GetComponent<TextMeshProUGUI>().text = $"{Currency}";
    }

    private void SaveIfNeeded()
    {
        if (!shouldSave)
            return;

        shouldSave = false;
        Save();
    }

    public void Save() => Sijil.Save(this, currencyKey, Currency);
}