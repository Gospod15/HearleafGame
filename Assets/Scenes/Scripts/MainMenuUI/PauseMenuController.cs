using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Елементи")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    void Start()
    {
        // 1. Примусово запускаємо час (щоб гра ожила після перезапуску)
        Time.timeScale = 1f; 
        isPaused = false;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Кнопка ESC
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Якщо відкритий інвентар - закриваємо його
            GameObject inventory = GameObject.Find("Inventory"); 
            if (inventory != null && inventory.activeSelf)
            {
                inventory.SetActive(false);
                return;
            }

            // Перемикаємо паузу
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0.1f; // Зупиняємо час
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f; // Відновлюємо час
        isPaused = false;
    }

    public void OnSaveButtonClicked()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveGame();
        }
    }

    public void OnExitButtonClicked()
    {
        Time.timeScale = 1f; // Обов'язково вмикаємо час перед виходом!
        SceneManager.LoadScene("MainMenu"); 
    }
}