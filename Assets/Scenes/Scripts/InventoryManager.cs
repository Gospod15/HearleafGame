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
    }

    public bool Add(ItemData item)
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

        // 2. Сценарій: Предмет новий або не стакається, або стак повний
        // Шукаємо перший порожній слот
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

