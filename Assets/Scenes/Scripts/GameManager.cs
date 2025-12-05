using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;


    public string playername;

    void Awake()
    {
        
        if (instance == null)
        {
            
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame(string namefrommenu)
    {
        this.playername = namefrommenu;
        // this.currentsave = new SaveData(); 

        SceneManager.LoadScene("MainGame");
    }

    public void LoadGame()
    {
        // SceneManager.LoadScene("GameScene");
    }
}