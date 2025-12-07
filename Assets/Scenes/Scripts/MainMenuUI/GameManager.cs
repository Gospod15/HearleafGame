using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Налаштування")]
    public string currentPlayerName; // Ім'я, яке ввів гравець
    public string currentSaveFileName; // Назва файлу (save_Dimas.json)

    [Header("База предметів (Заповни в інспекторі!)")]
    public List<ItemData> allGameItems; 

    // Дані для завантаження в гру
    public SaveData currentLoadedData; 

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // --- 1. ПОЧАТОК НОВОЇ ГРИ ---
    public void StartNewGame(string nameFromInput)
    {
        currentPlayerName = nameFromInput;
        
        // Формуємо ім'я майбутнього файлу, але ЩЕ НЕ ЗБЕРІГАЄМО
        currentSaveFileName = $"save_{currentPlayerName}.json";
        
        // Очищаємо дані, це нова гра
        currentLoadedData = null; 
        
        SceneManager.LoadScene("MainGame"); 
    }

    // --- 2. ЗАВАНТАЖЕННЯ (З меню) ---
    public void LoadGame(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentLoadedData = JsonUtility.FromJson<SaveData>(json);
            
            // Відновлюємо ім'я та файл
            currentPlayerName = currentLoadedData.saveName;
            currentSaveFileName = fileName;

            SceneManager.LoadScene("MainGame"); 
        }
        else
        {
            Debug.LogError("Файл не знайдено: " + path);
        }
    }

    // --- 3. ЗБЕРЕЖЕННЯ (В грі) ---
    public void SaveGame()
    {
        SaveData data = new SaveData();

        data.saveName = currentPlayerName;

        if (WorldGenerator.instance != null)
            data.worldSeed = WorldGenerator.instance.seed;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            data.currentHealth = player.currentHealth;
            data.currentFeed = player.currentFeed;
            data.currentStamina = player.currentStamina;
            data.playerPosition[0] = player.transform.position.x;
            data.playerPosition[1] = player.transform.position.y;
            data.playerPosition[2] = player.transform.position.z;
        }

        if (InventoryManager.instance != null)
        {
            foreach (var slot in InventoryManager.instance.slots)
            {
                if (slot.item != null && slot.amount > 0)
                {
                    InventoryItemData itemData = new InventoryItemData();
                    itemData.itemName = slot.item.itemName;
                    itemData.amount = slot.amount;
                    itemData.isShopSlot = slot.isShopSlot;
                    data.inventory.Add(itemData);
                }
            }
        }

        // Записуємо файл тільки зараз!
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, currentSaveFileName);
        File.WriteAllText(path, json);
        
        Debug.Log($"Гру збережено! Слот створено: {currentSaveFileName}");
    }

    public ItemData GetItemByName(string name)
    {
        return allGameItems.FirstOrDefault(i => i.itemName == name);
    }
}

// --- КЛАСИ ДАНИХ ---
[System.Serializable]
public class SaveData
{
    public string saveName;
    public int worldSeed;
    public float[] playerPosition = new float[3];
    public float currentHealth;
    public float currentFeed;
    public float currentStamina;
    public List<InventoryItemData> inventory = new List<InventoryItemData>();
}

[System.Serializable]
public class InventoryItemData
{
    public string itemName;
    public int amount;
    public bool isShopSlot;
}