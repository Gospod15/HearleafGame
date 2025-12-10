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
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            OnDropClicked();
        }

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
            ItemName.text =  currentItem.itemName;
        } 
        else 
        {
            ClearSelection();
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
            Vector3 dropPos = player.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
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
            ItemName.text = currentItem.itemName; 
        }
    }

    public void ClearSelection()
    {
        currentItem = null;
        currentSlot = null;
        ItemName.text = ""; 
    }
}