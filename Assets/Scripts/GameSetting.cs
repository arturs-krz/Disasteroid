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
        PlayerPrefs.SetString("Username", usernameInputField.text);
        SceneManager.LoadScene("Disasteroid");
    }

    void Start()
    {
        ActiveInitPanel();
    }

    public void ActiveInitPanel()
    {
        InitSubPanel.SetActive(true);
        StartSubPanel.SetActive(false);
    }

    public void ActiveStartPanel()
    {
        InitSubPanel.SetActive(false);
        StartSubPanel.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
