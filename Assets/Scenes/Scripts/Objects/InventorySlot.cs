using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // 1. ВАЖНО: Добавь эту библиотеку

// 2. Добавь интерфейс IPointerClickHandler
public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image iconComponent;
    public TextMeshProUGUI amountText;
    
    public ItemData item;
    public int amount;

    public void AddItem(ItemData newItem, int count)
    {
        item = newItem;
        amount = count;
        iconComponent.sprite = item.icon;
        iconComponent.enabled = true;

        if (item.isStackable && amount > 1)
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

    // 3. Реализуем метод клика
    public void OnPointerClick(PointerEventData eventData)
    {
        // Если слот пустой - ничего не делаем
        if (item == null) return;

        // Если нажата ПРАВАЯ кнопка мыши (Right Button)
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Открываем наше меню
            if (ContextMenuController.instance != null)
            {
<<<<<<< Updated upstream
                ContextMenuController.instance.OpenMenu(item, this);
=======
                player.Eat(item.FeedAmount);
                
                amount--;
                if (amount <= 0) {
                    ClearSlot();
                    if (ContextMenuController.instance != null) ContextMenuController.instance.ClearSelection();
                } else {
                    AddItem(item, amount);
                }
>>>>>>> Stashed changes
            }
        }
    }
}