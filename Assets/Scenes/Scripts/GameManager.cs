using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Settings")]
    public string currentPlayerName = "Player";
    public string currentSaveFileName = "save_Player.json";

    [Header("Item Database")]
    public List<ItemData> allGameItems;

    public SaveData currentLoadedData;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        if (SceneData.isNewGame)
        {
            // NEW GAME
            currentPlayerName = !string.IsNullOrEmpty(SceneData.playerNameToLoad) ? SceneData.playerNameToLoad : "Player";
            currentSaveFileName = $"save_{currentPlayerName}.json";

            if (WorldGenerator.instance != null)
            {
                WorldGenerator.instance.seed = Random.Range(-10000, 10000);
                WorldGenerator.instance.GenerateWorld();
            }
        }
        else if (!string.IsNullOrEmpty(SceneData.saveFileName))
        {
            LoadGame(SceneData.saveFileName);
        }
        else
        {
            if (string.IsNullOrEmpty(currentSaveFileName))
            {
                currentPlayerName = "DebugPlayer";
                currentSaveFileName = "save_DebugPlayer.json";
            }

            if (WorldGenerator.instance != null)
            {
                WorldGenerator.instance.seed = 12345;
                WorldGenerator.instance.GenerateWorld();
            }
        }

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            InitializePlayer(player);
        }
    }

    public void InitializePlayer(PlayerController player)
    {
        bool loadedFromSave = false;

        if (currentLoadedData != null)
        {
            Vector3 pos = new Vector3(
                currentLoadedData.playerPosition[0],
                currentLoadedData.playerPosition[1],
                currentLoadedData.playerPosition[2]
            );

            if (pos != Vector3.zero) player.transform.position = pos;

            player.currentHealth = currentLoadedData.currentHealth;
            player.currentFeed = currentLoadedData.currentFeed;
            player.currentStamina = currentLoadedData.currentStamina;

            loadedFromSave = true;
        }

        if (!loadedFromSave || player.currentHealth <= 1f || player.currentStamina <= 1f)
        {
            player.currentHealth = player.maxHealth;
            player.currentFeed = player.maxFeed;
            player.currentStamina = player.maxStamina;
        }

    }

    public void LoadGame(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            currentLoadedData = JsonUtility.FromJson<SaveData>(json);

            currentPlayerName = currentLoadedData.saveName;
            currentSaveFileName = fileName;

            if (SceneManager.GetActiveScene().name != "MainGame")
            {
                SceneManager.LoadScene("MainGame");
            }
            else
            {
                InitializePlayer(FindObjectOfType<PlayerController>());
            }
        }
        else
        {
            Debug.LogError("Шлях не знайдено: " + path);
        }
    }

    public void SaveGame()
    {
        if (string.IsNullOrEmpty(currentSaveFileName))
        {
            currentSaveFileName = string.IsNullOrEmpty(currentPlayerName) ? "save_Unknown.json" : $"save_{currentPlayerName}.json";
        }

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
                    data.inventory.Add(itemData);
                }
            }
        }

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, currentSaveFileName);

        try
        {
            File.WriteAllText(path, json);
            Debug.Log($"Гра збережена! {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Помилка збереженгя: {e.Message}");
        }
    }

    public ItemData GetItemByName(string name)
    {
        return allGameItems.FirstOrDefault(i => i.itemName == name);
    }
}

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