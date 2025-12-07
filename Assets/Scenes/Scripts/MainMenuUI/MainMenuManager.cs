using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    [Header("Нова Гра")]
    public TMP_InputField nameInputField; // Поле вводу імені

    [Header("Завантаження")]
    public Transform slotsContainer;      // Content всередині ScrollView
    public GameObject slotPrefab;         // Префаб кнопки (Slot 1)
    public Button loadButton;             // Кнопка "Завантажити"
    public Button deleteButton;           // Кнопка "Видалити" (якщо є)

    private SaveSlotUI selectedSlot;      // Який слот зараз зелений

    void Start()
    {
        loadButton.interactable = false; // Вимикаємо Load, поки не вибрали слот
        if (deleteButton) deleteButton.interactable = false;
        
        LoadSlotsFromFolder(); // Скануємо папку
    }

    // --- 1. Кнопка START GAME ---
    public void OnStartGamePressed()
    {
        string name = nameInputField.text;
        if (!string.IsNullOrEmpty(name))
        {
            GameManager.instance.StartNewGame(name);
        }
        else
        {
            Debug.Log("Введіть ім'я!");
        }
    }

    // --- 2. Сканування папки та створення кнопок ---
    void LoadSlotsFromFolder()
    {
        // Спочатку видаляємо старі кнопки (щоб не дублювались)
        foreach (Transform child in slotsContainer) Destroy(child.gameObject);

        // Шукаємо всі файли, що починаються на "save_"
        string[] files = Directory.GetFiles(Application.persistentDataPath, "save_*.json");

        foreach (string filePath in files)
        {
            // Читаємо файл, щоб дізнатися ім'я гравця всередині
            string json = File.ReadAllText(filePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Створюємо нову кнопку в меню
            GameObject newObj = Instantiate(slotPrefab, slotsContainer);
            SaveSlotUI slotUI = newObj.GetComponent<SaveSlotUI>();

            // Налаштовуємо її (передаємо ім'я файлу та ім'я гравця)
            string fileName = Path.GetFileName(filePath);
            slotUI.Setup(fileName, data.saveName, this);
        }
    }

    // --- 3. Логіка вибору (Зелений колір) ---
    public void OnSlotSelected(SaveSlotUI slot)
    {
        // Якщо до цього щось було вибране - робимо його білим
        if (selectedSlot != null) selectedSlot.SetVisualSelected(false);

        // Запам'ятовуємо новий слот і робимо зеленим
        selectedSlot = slot;
        selectedSlot.SetVisualSelected(true);

        // Вмикаємо кнопку Load
        loadButton.interactable = true;
        if (deleteButton) deleteButton.interactable = true;
    }

    // --- 4. Кнопка LOAD ---
    public void OnLoadGamePressed()
    {
        if (selectedSlot != null)
        {
            GameManager.instance.LoadGame(selectedSlot.fileName);
        }
    }

    // --- 5. Кнопка DELETE (Опціонально) ---
    public void OnDeletePressed()
    {
        if (selectedSlot != null)
        {
            string path = Path.Combine(Application.persistentDataPath, selectedSlot.fileName);
            File.Delete(path); // Видаляємо файл
            
            // Оновлюємо список
            LoadSlotsFromFolder();
            selectedSlot = null;
            loadButton.interactable = false;
        }
    }
}