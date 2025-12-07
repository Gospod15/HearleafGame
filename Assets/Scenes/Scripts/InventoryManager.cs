using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Transform slotsParent;
    public InventorySlot[] slots;

    // Список нам тут вже менше потрібен, будемо працювати прямо зі слотами
    // Але для зручності можна залишити, проте логіку краще будувати на перевірці слотів.

    public ItemData testItem;

    void Awake() { instance = this; }


    void Start()
    {
        slots = slotsParent.GetComponentsInChildren<InventorySlot>();

        // --- [NEW] ЗАВАНТАЖЕННЯ ІНВЕНТАРЯ ---
        if (GameManager.instance != null && GameManager.instance.currentLoadedData != null)
        {
            LoadInventory(GameManager.instance.currentLoadedData.inventory);
        }
    }
    public void LoadInventory(List<InventoryItemData> savedInventory)
    {
        // 1. Очищаємо інвентар перед завантаженням
        foreach (var slot in slots) 
        {
            slot.ClearSlot();
        }

        // 2. Проходимо по збережених даних
        int slotIndex = 0;
        foreach (var savedItem in savedInventory)
        {
            if (slotIndex >= slots.Length) break;

            // Просимо GameManager знайти справжній ItemData за назвою
            ItemData realItem = GameManager.instance.GetItemByName(savedItem.itemName);

            if (realItem != null)
            {
                slots[slotIndex].AddItem(realItem, savedItem.amount);
                // Якщо треба відновити, чи це слот магазину (хоча зазвичай магазин не зберігається в інвентарі гравця)
                slots[slotIndex].isShopSlot = savedItem.isShopSlot; 
            }
            slotIndex++;
        }
        
        RefreshUI(); // Оновлюємо картинку
    }

<<<<<<< Updated upstream
    public bool Add(ItemData item)
=======
    public void RefreshUI()
    {
        if (slots == null) return;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null)
            {
                slot.AddItem(slot.item, slot.amount);
            }
            else
            {
                slot.ClearSlot();
            }
        }
    }

    public bool AddItem(ItemData item, int amount)
>>>>>>> Stashed changes
    {
        // 1. Сценарій: Предмет стакається (яблука, стріли)
        if (item.isStackable)
        {
            // Шукаємо слот, де ВЖЕ є цей предмет і де є місце (< maxStackSize)
            foreach (InventorySlot slot in slots)
            {
                if (slot.item == item && slot.amount < item.maxStackSize)
                {
                    slot.amount++; // Збільшуємо кількість
                    slot.AddItem(item, slot.amount); // Оновлюємо вигляд
                    return true;
                }
            }
        }

<<<<<<< Updated upstream
        // 2. Сценарій: Предмет новий або не стакається, або стак повний
        // Шукаємо перший порожній слот
=======
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
            return false;
        }
        
        return true;
    }

    public int GetItemAmount(ItemData itemToCheck)
    {
        int total = 0;
>>>>>>> Stashed changes
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == null) // Якщо слот пустий
            {
                slot.AddItem(item, 1); // Додаємо 1 штуку
                return true;
            }
        }

        Debug.Log("Інвентар повний!");
        return false;
    }
    

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.kKey.wasPressedThisFrame) 
        {
            Add(testItem);
        }
    }

}

