using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ShopItem
{
    public ItemData item;
    public int amount;
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("Економіка")]
    public ItemData currencyItem; 

    [Header("UI Елементи")]
    public GameObject shopUI;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI feedbackText; 

    [Header("Товари")]
    public Transform slotsParent;
    public List<ShopItem> itemsForSale; 

    private InventorySlot[] shopSlots;

    void Awake() { instance = this; }

    void Start()
    {
        shopSlots = slotsParent.GetComponentsInChildren<InventorySlot>();
        CloseShop();
        SetupShop();
    }

    void SetupShop()
    {
        for (int i = 0; i < shopSlots.Length; i++)
        {
            shopSlots[i].isShopSlot = true; 

            if (i < itemsForSale.Count)
            {
                shopSlots[i].AddItem(itemsForSale[i].item, itemsForSale[i].amount);
            }
            else
            {
                shopSlots[i].ClearSlot();
            }
        }
    }

    void AddEarningsToShop(ItemData moneyItem, int amount)
    {
        for (int i = shopSlots.Length - 1; i >= 0; i--)
        {
            if (shopSlots[i].item == moneyItem && shopSlots[i].amount < moneyItem.maxStackSize)
            {
                int space = moneyItem.maxStackSize - shopSlots[i].amount;
                int toAdd = Mathf.Min(space, amount);

                shopSlots[i].amount += toAdd;
                shopSlots[i].AddItem(moneyItem, shopSlots[i].amount);

                amount -= toAdd;
                if (amount <= 0) return;
            }
        }

        if (amount > 0)
        {
            for (int i = shopSlots.Length - 1; i >= 0; i--)
            {
                if (shopSlots[i].item == null)
                {
                    int toAdd = Mathf.Min(moneyItem.maxStackSize, amount);
                    
                    shopSlots[i].isShopSlot = true;
                    shopSlots[i].AddItem(moneyItem, toAdd);

                    amount -= toAdd;
                    if (amount <= 0) return;
                }
            }
        }
    }

    public void TryBuyItem(InventorySlot slot)
    {
        if (slot.item == null || slot.amount <= 0)
        {
            ShowFeedback("Товар закінчився!", Color.red);
            return;
        }

        int price = slot.item.price;
        int playerMoney = InventoryManager.instance.GetItemAmount(currencyItem);

        if (playerMoney >= price)
        {
            bool added = InventoryManager.instance.AddItem(slot.item, 1);

            if (added)
            {
                InventoryManager.instance.RemoveItem(currencyItem, price);

                AddEarningsToShop(currencyItem, price);
                slot.amount--; 
                
                if (slot.amount > 0)
                    slot.AddItem(slot.item, slot.amount);
                else
                    slot.ClearSlot(); 

                ShowFeedback("Куплено!", Color.green);
                
                if (slot.amount > 0) SelectShopItem(slot.item);
                else ClearInfo();
            }
            else
            {
                ShowFeedback("Інвентар повний!", Color.red);
            }
        }
        else
        {
            ShowFeedback($"Треба {price} монет!", Color.red);
        }
    }

    public void SelectShopItem(ItemData item)
    {
        itemNameText.text = item.itemName;
        itemPriceText.text = $"{item.price} монет";
        feedbackText.text = "";
    }

    void ClearInfo()
    {
        itemNameText.text = "";
        itemPriceText.text = "";
        feedbackText.text = "";
    }

    void ShowFeedback(string text, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = text;
            feedbackText.color = color;
        }
    }

    public void OpenShop() { shopUI.SetActive(true); ClearInfo(); }
    public void CloseShop() { shopUI.SetActive(false); }
}