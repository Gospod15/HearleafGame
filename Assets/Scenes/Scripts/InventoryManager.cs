using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Transform slotsParent;
    public InventorySlot[] slots;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Знаходимо всі слоти в дочірніх об'єктах
        slots = slotsParent.GetComponentsInChildren<InventorySlot>();
    }

    // --- 1. ДОДАВАННЯ ПРЕДМЕТА ---
    // Я перейменував метод з Add на AddItem, щоб він збігався з ShopManager
    public bool AddItem(ItemData item, int amount)
    {
        // Сценарій 1: Предмет стакається
        if (item.isStackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.item == item && slot.amount < item.maxStackSize)
                {
                    int spaceInSlot = item.maxStackSize - slot.amount;
                    int amountToAdd = Mathf.Min(spaceInSlot, amount);

                    slot.amount += amountToAdd;
                    slot.AddItem(item, slot.amount); // Оновлюємо візуал
                    
                    amount -= amountToAdd;
                    if (amount <= 0) return true;
                }
            }
        }

        // Сценарій 2: Шукаємо порожній слот
        if (amount > 0)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.item == null)
                {
                    int amountToAdd = Mathf.Min(item.maxStackSize, amount);
                    slot.AddItem(item, amountToAdd);
                    
                    amount -= amountToAdd;
                    if (amount <= 0) return true;
                }
            }
        }

        if (amount > 0)
        {
            Debug.Log("Інвентар повний!");
            return false;
        }
        
        return true;
    }

    // --- 2. ПЕРЕВІРКА КІЛЬКОСТІ (ГРОШЕЙ) ---
    // Цього методу у тебе не було!
    public int GetItemAmount(ItemData itemToCheck)
    {
        int total = 0;
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null && slot.item == itemToCheck)
            {
                total += slot.amount;
            }
        }
        // Debug.Log($"Перевіряємо {itemToCheck.itemName}: знайдено {total}");
        return total;
    }

    // --- 3. ВИДАЛЕННЯ ПРЕДМЕТА (ОПЛАТА) ---
    // Цього методу теж не було!
    public void RemoveItem(ItemData itemToRemove, int amountToRemove)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToRemove)
            {
                if (slot.amount >= amountToRemove)
                {
                    slot.amount -= amountToRemove;
                    amountToRemove = 0;
                }
                else
                {
                    amountToRemove -= slot.amount;
                    slot.amount = 0;
                }

                // Оновлюємо вигляд слота
                if (slot.amount <= 0)
                {
                    slot.ClearSlot();
                }
                else
                {
                    slot.AddItem(slot.item, slot.amount);
                }

                if (amountToRemove <= 0) return;
            }
        }
    }
}