using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro; // <--- ВАЖНО: Добавлена эта библиотека!
public class ContextMenuController : MonoBehaviour
{
    public static ContextMenuController instance;

    public TextMeshProUGUI ItemName; 
    public TextMeshProUGUI ItemDescription;
    public GameObject menuObj;
    public Button useButton; 
    public Button sellButton; 
    public Button dropButton;

    private ItemData currentItem; // Какой предмет мы сейчас выбрали
    private InventorySlot currentSlot; // Из какого слота

    void Awake()
    {
        instance = this;
        menuObj.SetActive(false); // Скрываем при старте
        
        // Подписываем кнопку на действие
        dropButton.onClick.AddListener(OnDropClicked);
    }

    void Update()
    {
        // Если меню открыто и мы кликаем левой кнопкой МИМО меню - закрываем его
        if (menuObj.activeSelf && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Небольшая задержка или проверка UI нужна, но для простоты закроем
            // Если вы нажмете на кнопку, сработает кнопка. Если мимо - закроется в след кадре.
            // Для идеальной работы лучше использовать EventSystem, но пока так:
             // CloseMenu(); // (раскомментируй, если будет мешать нажимать кнопку)
        }
    }

    // Открыть меню в позиции мыши
    public void OpenMenu(ItemData item, InventorySlot slot)
    {
        currentItem = item;
        currentSlot = slot;

<<<<<<< Updated upstream
        menuObj.SetActive(true);
        
        // Перемещаем меню к мышке
        
        float mouseX = Mouse.current.position.ReadValue().x;
        float mouseY = Mouse.current.position.ReadValue().y;

        menuObj.transform.position =  new Vector3(mouseX + 120,mouseY+120,0);

        ItemName.text = currentItem.itemName.ToString();
        ItemDescription.text = "Description: " + currentItem.description.ToString();
=======
        if (currentItem != null)
        {
            ItemName.text = currentItem.itemName;
        } 
        else 
        {
            ClearSelection();
        }
>>>>>>> Stashed changes
    }

    public void CloseMenu()
    {
        menuObj.SetActive(false);
    }

    // Логика кнопки "Выбросить"
    void OnDropClicked()
    {
        if (currentItem != null && currentSlot != null)
        {
            DropItem();
            CloseMenu();
        }
    }

    void DropItem()
    {
        // 1. Спавним предмет в мире рядом с игроком
        // Нам нужно найти игрока. Можно через FindObject или Singleton GameManager
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (currentItem.dropPrefab != null)
        {
            // Спавним чуть правее игрока
            Vector3 dropPos = player.position + new Vector3(1, 0, 0); 
            Instantiate(currentItem.dropPrefab, dropPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("У предмета нет Drop Prefab! Он просто исчез.");
        }

        // 2. Удаляем 1 штуку из инвентаря
        currentSlot.amount--;
        
        if (currentSlot.amount <= 0)
        {
            // Если предметов стало 0 - очищаем слот полностью
            currentSlot.ClearSlot();
            // Также нужно удалить из списка в InventoryManager, но это сложнее,
            // пока просто очистим слот визуально.
            // Для полной синхронизации лучше добавить метод Remove в InventoryManager.
        }
        else
        {
            // Если предметов много - просто обновляем текст
            currentSlot.AddItem(currentItem, currentSlot.amount);
<<<<<<< Updated upstream
=======
            ItemName.text = currentItem.itemName; 
>>>>>>> Stashed changes
        }
    }
}