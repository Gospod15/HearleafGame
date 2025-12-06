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
    
    // Флаг: чи це слот магазину?
    public bool isShopSlot = false; 

    public void AddItem(ItemData newItem, int count)
    {
        item = newItem;
        amount = count;
        iconComponent.sprite = item.icon;
        iconComponent.enabled = true;

        // В магазині показуємо кількість завжди (щоб бачити залишок товару)
        // В інвентарі - тільки якщо стакається
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
            // === ЛОГІКА МАГАЗИНУ ===
            if (isShopSlot)
            {
                if (ShopManager.instance != null && item != null)
                {
                    // 1. Показуємо ціну
                    ShopManager.instance.SelectShopItem(item);
                    // 2. Пробуємо купити 1 штуку
                    ShopManager.instance.TryBuyItem(this);
                }
                return; // Виходимо, щоб не відкрити меню викидання
            }

            // === ЛОГІКА ІНВЕНТАРЮ (твоя стара) ===
            if (ContextMenuController.instance != null)
            {
                if (item != null)
                    ContextMenuController.instance.OpenMenu(item, this);
                else
                    ContextMenuController.instance.ClearSelection();
            }
        }
    }
}