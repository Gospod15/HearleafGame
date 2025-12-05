using UnityEngine;
using UnityEngine.UI;

using TMPro;


public class MenuController : MonoBehaviour
{

    public TMP_InputField inputfield;


    public void OnNewGameButtonClick()
    {

        string playername = inputfield.text;


        if (string.IsNullOrEmpty(playername))
        {
            Debug.Log("Треба ввести ім'я!");
            return; 
        }

        GameManager.instance.StartNewGame(playername);
    }

    public void OnLoadGameButtonClick()
    {

        // GameManager.instance.LoadGame();
    }

}

