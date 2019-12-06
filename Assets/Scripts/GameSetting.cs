using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject InitSubPanel;
    public GameObject StartSubPanel;

    public InputField usernameInputField;

    public void StartGame()
    {
        SceneManager.LoadScene("Disasteroid");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
