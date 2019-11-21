using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public Slider PD;
    public Slider WL;
    public Slider CO2;
    public Text userDisplay;
    public GameObject gameover;
    // Start is called before the first frame update
    void Start()
    {
        gameover.SetActive(false);
        if (PlayerPrefs.GetString("Username") == "")
            PlayerPrefs.SetString("Username", "John Doe");
        userDisplay.text = PlayerPrefs.GetString("Username").ToString() + " is playing!";
        PD.value = PD.minValue;
        WL.value = PD.minValue;
        CO2.value = PD.minValue;
    }

    public void BombAttack()
    {
        PD.value += PD.maxValue / 10;
        CO2.value += CO2.maxValue / 5;
    }

    public void BacktoMain()
    {
        SceneManager.LoadScene("GameStart");
    }

    public void GameOver()
    {
        gameover.SetActive(true);
        SceneManager.LoadScene("GameStart");
    }

    // Update is called once per frame
    void Update()
    {
        WL.value += Time.deltaTime * CO2.value / 10;
        PD.value += Time.deltaTime * CO2.value / 30;
        CO2.value += Time.deltaTime * (PD.maxValue - PD.value) / 30;
        if (PD.value >= PD.maxValue || WL.value >= WL.maxValue || CO2.value >= CO2.maxValue)
            GameOver();
    }
}
