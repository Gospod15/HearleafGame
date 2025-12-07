using UnityEngine;

public class ItemInWorldManager : MonoBehaviour
{
    public ItemData itemData; 
    public int amount = 1;

    public void Pickup()
    {
        if (InventoryManager.instance != null)
        {
            bool added = InventoryManager.instance.AddItem(itemData, amount);
            
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