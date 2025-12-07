using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; 

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image iconComponent;
    public TextMeshProUGUI amountText;
    
    public ItemData item;
    public int amount;
    
    public bool isShopSlot = false; 

    private float lastClickTime; 
    private const float DOUBLE_CLICK_SPEED = 0.3f; 

    public void AddItem(ItemData newItem, int count)
    {
        item = newItem;
        amount = count;
        iconComponent.sprite = item.icon;
        iconComponent.enabled = true;

        if (isShopSlot || (item.isStackable && amount > 1))
        {
            amountText.text = amount.ToString();
            amountText.enabled = true;
        }
        else
        {
            amountText.enabled = false;
        }
    }

    public void ClearSlot()
    {
        item = null;
        amount = 0;
        iconComponent.sprite = null;
        iconComponent.enabled = false;
        if (amountText != null) amountText.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            bool isDoubleClick = timeSinceLastClick <= DOUBLE_CLICK_SPEED;

            if (isShopSlot)
            {
                if (ShopManager.instance != null && item != null)
                {
                    if (isDoubleClick)
                    {
                        ShopManager.instance.TryBuyItem(this);
                    }
                    else
                    {
                        ShopManager.instance.SelectShopItem(item);
                    }
                }
            }
            else 
            {
                if (isDoubleClick)
                {
                    TryUseItem(); 
                }
                else
                {
                    if (ContextMenuController.instance != null)
                    {
                        if (item != null)
                            ContextMenuController.instance.OpenMenu(item, this);
                        else
                            ContextMenuController.instance.ClearSelection();
                    }
                }
            }

            lastClickTime = Time.time;
        }
    }

    private void TryUseItem()
    {
        if (item == null) return;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        if (item.itemType == ItemType.Food)
        {
            if (item.isEatable)
            {
                player.Eat(item.FeedAmount);
                Debug.Log($"З'їв {item.itemName}");
                
                amount--;
                if (amount <= 0) {
                    ClearSlot();
                    if (ContextMenuController.instance != null) ContextMenuController.instance.ClearSelection();
                } else {
                    AddItem(item, amount);
                }
            }
        }
        else if (item.itemType == ItemType.Tool)
        {
            player.EquipItem(item);
            
        }
    }
}