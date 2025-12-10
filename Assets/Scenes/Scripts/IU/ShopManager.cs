using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ShopItem
{
    public ItemData item;
    public int amount;
    public int LocalPrice = 0; 
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("Валюта")]
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
            shopSlots[i].slotIndex = i;

            if (i < itemsForSale.Count)
            {
                UpdateSlotUI(i);
            }
            else
            {
                shopSlots[i].ClearSlot();
            }
        }
    }

    int GetPrice(int index)
    {
        if (index < 0 || index >= itemsForSale.Count) return 0;
        ShopItem shopEntry = itemsForSale[index];
        return (shopEntry.LocalPrice > 0) ? shopEntry.LocalPrice : shopEntry.item.Sellprice;
    }

    void UpdateSlotUI(int index)
    {
        if (index < 0 || index >= itemsForSale.Count) return;
        ShopItem shopEntry = itemsForSale[index];
        shopSlots[index].AddItem(shopEntry.item, shopEntry.amount);
    }
    void AddEarningsToShop(int amountToAdd)
    {
        if (currencyItem == null || amountToAdd <= 0) return;

        for (int i = 0; i < itemsForSale.Count; i++)
        {
            if (itemsForSale[i].item == currencyItem)
            {
                itemsForSale[i].amount += amountToAdd;
                UpdateSlotUI(i);
                return;
            }
        }

        if (itemsForSale.Count < shopSlots.Length)
        {
            ShopItem newCoins = new ShopItem();
            newCoins.item = currencyItem;
            newCoins.amount = amountToAdd;
            newCoins.LocalPrice = 1;

            itemsForSale.Add(newCoins);
            UpdateSlotUI(itemsForSale.Count - 1);
        }
    }

    public void TryBuyItem(int index)
    {
        if (index < 0 || index >= itemsForSale.Count) return;

        ShopItem shopEntry = itemsForSale[index];
        
        if (shopEntry.amount <= 0) 
        {
            return;
        }

        if (shopEntry.item == currencyItem)
        {
             return;
        }

        int price = GetPrice(index);
        
        if(currencyItem == null) return;

        int playerMoney = InventoryManager.instance.GetItemAmount(currencyItem);

        if (shopEntry.item.CanBuy) 
        {
            if (playerMoney >= price) 
            {
                bool added = InventoryManager.instance.AddItem(shopEntry.item, 1);

                if (added) 
                {
                    InventoryManager.instance.RemoveItem(currencyItem, price);
                    
                    shopEntry.amount--; 
                    
                    AddEarningsToShop(price);
                    
                    if (shopEntry.amount > 0)
                    {
                        UpdateSlotUI(index);
                        SelectShopItem(index);
                    }
                    else
                    {
                        shopSlots[index].ClearSlot();
                        ClearInfo();
                    }
                } 
                else 
                {
                }
            } 
            else 
            {
            }
        }
    }

    public void SelectShopItem(int index)
    {
        if (index < 0 || index >= itemsForSale.Count) return;
        
        ShopItem shopEntry = itemsForSale[index];
        int price = GetPrice(index);

        itemNameText.text = shopEntry.item.itemName;
        itemPriceText.text = $"{price} монет";
        
    }

    void ClearInfo()
    {
        itemNameText.text = "";
        itemPriceText.text = "";
    }

    public void OpenShop() { shopUI.SetActive(true); ClearInfo(); }
    public void CloseShop() { shopUI.SetActive(false); }
}