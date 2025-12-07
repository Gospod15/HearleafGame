using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI nameText; // Сюди перетягни текст на кнопці
    public Image backgroundImage;    // Сюди перетягни саму картинку кнопки

    [HideInInspector] public string fileName; // Тут буде save_Alex.json
    private MainMenuManager menuManager;

    // Цей метод викликає Меню при створенні кнопки
    public void Setup(string file, string playerName, MainMenuManager manager)
    {
        fileName = file;
        nameText.text = playerName; // Пишемо ім'я гравця
        menuManager = manager;

        // Додаємо подію кліку
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        // Кажемо менеджеру меню: "Мене клікнули!"
        menuManager.OnSlotSelected(this);
    }

    // Метод для зміни кольору
    public void SetVisualSelected(bool isSelected)
    {
        if (isSelected)
            backgroundImage.color = Color.green; // Зелений, якщо вибрали
        else
            backgroundImage.color = Color.white; // Білий, якщо ні
    }
}