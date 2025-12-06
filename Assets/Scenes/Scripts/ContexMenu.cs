using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; 
using TMPro;

public class ContextMenuController : MonoBehaviour
{
    public static ContextMenuController instance;
    public Image InventoryMenu;     
    public TextMeshProUGUI ItemName;
    
    private ItemData currentItem;   
    private InventorySlot currentSlot; 

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // 1. Дроп предмета на G
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            OnDropClicked();
        }

        // 2. ИСПРАВЛЕНИЕ: Очистка текста, если меню закрылось
        // Сначала проверяем, что ссылка есть (!= null), а потом проверяем активность
        if (!InventoryMenu.gameObject.activeSelf)
        {
            ClearSelection();
            ItemName.text = ""; 
        }
    }

    public void OpenMenu(ItemData item, InventorySlot slot)
    {
        currentItem = item;
        currentSlot = slot;

        if (currentItem != null)
        {
            ItemName.text = "Selected: " + currentItem.itemName;
        } 
        else 
        {
            ClearSelection(); // Если передали null, просто очищаем
        }
    }

    void OnDropClicked()
    {
        if (currentItem != null && currentSlot != null)
        {
            DropItem();
        }
    }

    void DropItem()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        if (currentItem.dropPrefab != null)
        {
            Vector3 dropPos = player.position + new Vector3(1, 0, 0); 
            Instantiate(currentItem.dropPrefab, dropPos, Quaternion.identity);
        }

        currentSlot.amount--;

        if (currentSlot.amount <= 0)
        {
            currentSlot.ClearSlot();
            ClearSelection(); 
        }
        else
        {
            currentSlot.AddItem(currentItem, currentSlot.amount);
            ItemName.text = "Selected: " + currentItem.itemName; 
        }
    }

    // Сделали метод public, чтобы его можно было вызвать из слота (если нужно)
    public void ClearSelection()
    {
        currentItem = null;
        currentSlot = null;
        ItemName.text = ""; 
    }
}