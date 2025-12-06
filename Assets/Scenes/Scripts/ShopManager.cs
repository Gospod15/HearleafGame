using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

// Допоміжний клас для налаштування товарів в Інспекторі
[System.Serializable]
public class ShopItem
{
    public ItemData item;
    public int amount; // Наприклад, продаємо 20 штук
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    [Header("Економіка")]
    public ItemData currencyItem; // <-- СЮДА ПЕРЕТЯГНИ ПРЕДМЕТ "МОНЕТА"

    [Header("UI Елементи")]
    public GameObject shopUI;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI feedbackText; // Текст для помилок ("Немає грошей")

    [Header("Товари")]
    public Transform slotsParent;
    public List<ShopItem> itemsForSale; // Використовуємо наш новий клас ShopItem

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
            shopSlots[i].isShopSlot = true; // Кажемо слоту, що він магазинний

            if (i < itemsForSale.Count)
            {
                // Завантажуємо товар і його кількість
                shopSlots[i].AddItem(itemsForSale[i].item, itemsForSale[i].amount);
            }
            else
            {
                shopSlots[i].ClearSlot();
            }
        }
    }

    // Головна логіка покупки
    public void TryBuyItem(InventorySlot slot)
    {
        // 1. Перевірки наявності товару
        if (slot.item == null || slot.amount <= 0)
        {
            ShowFeedback("Товар закінчився!", Color.red);
            return;
        }

        int price = slot.item.price;
        // 2. Перевірка грошей у гравця
        int playerMoney = InventoryManager.instance.GetItemAmount(currencyItem);

        if (playerMoney >= price)
        {
            // 3. Спроба додати гравцю (перевірка місця)
            // Додаємо 1 штуку
            bool added = InventoryManager.instance.AddItem(slot.item, 1);

            if (added)
            {
                // УСПІХ:
                // А. Забираємо гроші
                InventoryManager.instance.RemoveItem(currencyItem, price);
                
                // Б. Зменшуємо кількість у магазині
                slot.amount--; 
                
                // В. Оновлюємо вигляд слота в магазині
                if (slot.amount > 0)
                    slot.AddItem(slot.item, slot.amount);
                else
                    slot.ClearSlot(); // Якщо стало 0, видаляємо

                ShowFeedback("Куплено!", Color.green);
                
                // Оновлюємо інформацію про ціну/залишок
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