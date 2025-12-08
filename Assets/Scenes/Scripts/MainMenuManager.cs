using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Налаштування")]

    public string gameSceneName = "MainGame"; 

    [Header("Нова Гра")]
    public TMP_InputField nameInputField;

    [Header("Завантаження")]
    public Transform slotsContainer;
    public GameObject slotPrefab;
    public Button loadButton;
    public Button deleteButton;

    private SaveSlotUI selectedSlot; 

    void Start()
    {
        if (loadButton != null) loadButton.interactable = false;
        if (deleteButton != null) deleteButton.interactable = false;
        
        LoadSlotsFromFolder();
    }

    public void OnStartGamePressed()
    {

        string name = nameInputField.text;

        if (!string.IsNullOrEmpty(name))
        {
            SceneData.isNewGame = true;
            SceneData.playerNameToLoad = name;
            SceneData.saveFileName = "";

            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.Log("Введіть ім'я!");
        }
    }

    void LoadSlotsFromFolder()
    {
        foreach (Transform child in slotsContainer) Destroy(child.gameObject);

        if (!Directory.Exists(Application.persistentDataPath)) return;

        string[] files = Directory.GetFiles(Application.persistentDataPath, "save_*.json");

        foreach (string filePath in files)
        {
            try 
            {
                string json = File.ReadAllText(filePath);
                
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                if (data == null || string.IsNullOrEmpty(data.saveName))
                {
                    Debug.LogWarning($"Знайдено пошкоджений файл: {filePath}");
                    continue; 
                }

                GameObject newObj = Instantiate(slotPrefab, slotsContainer);
                SaveSlotUI slotUI = newObj.GetComponent<SaveSlotUI>();

                string fileName = Path.GetFileName(filePath);
                slotUI.Setup(fileName, data.saveName, this);
            }
            catch (System.Exception e)
            {
                File.Delete(filePath);
            }
        }
    }


    public void OnSlotSelected(SaveSlotUI slot)
    {

        if (selectedSlot != null) selectedSlot.SetVisualSelected(false);

        selectedSlot = slot;
        selectedSlot.SetVisualSelected(true);


        if (loadButton != null) loadButton.interactable = true;
        if (deleteButton != null) deleteButton.interactable = true;
    }

    public void OnLoadGamePressed()
    {
        if (selectedSlot != null)
        {
            SceneData.isNewGame = false;
            SceneData.saveFileName = selectedSlot.fileName;

            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void OnDeletePressed()
    {
        if (selectedSlot != null)
        {
            string path = Path.Combine(Application.persistentDataPath, selectedSlot.fileName);
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            
            LoadSlotsFromFolder();
            selectedSlot = null;
            
            if (loadButton != null) loadButton.interactable = false;
            if (deleteButton != null) deleteButton.interactable = false;
        }
    }
}