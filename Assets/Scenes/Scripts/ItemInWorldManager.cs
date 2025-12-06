using UnityEngine;

public class ItemInWorldManager : MonoBehaviour
{
    public ItemData itemData; // Сюди перетягни ScriptableObject (наприклад, StoneData) в інспекторі
    public int amount = 1;    // Скільки додавати (зазвичай 1)

    // Цей метод викличе гравець
    public void Pickup()
    {
        // Звертаємося до твого InventoryManager (переконайся, що він у тебе є)
        if (InventoryManager.instance != null)
        {
            // Спробуй додати предмет
            bool added = InventoryManager.instance.AddItem(itemData, amount);
            
            // Якщо місце було і предмет додався — знищуємо об'єкт зі сцени
            if (added)
            {
                Debug.Log($"Підібрано: {itemData.itemName}");
                Destroy(gameObject); 
            }
            else
            {
                Debug.Log("Інвентар повний!");
            }
        }
    }
}