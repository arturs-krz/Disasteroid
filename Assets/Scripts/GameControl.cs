using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public Slider PD;
    public Slider CO2;
    public GameObject gameover;

    //color
    public Color MaxColor;
    public Color MinColor;
    
    private Image populationSliderFill;
    private Image CO2SliderFill;

    private bool OpenWindow;
    int MaxVal = 100;

    // Start is called before the first frame update
    void Start()
    {
        OpenWindow = false;
    }

    public void BacktoMain()
    {
        SceneManager.LoadScene("GameStart");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Continue()
    {
        OpenWindow = false;
        gameover.SetActive(false);
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        gameover.SetActive(true);
        //Application.Quit(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OpenWindow = true;
        PD.value += Time.deltaTime * 2/100;
        Color colorPD = Color.Lerp(MinColor, MaxColor, (float)PD.value / MaxVal);
        PD.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = colorPD;

        Color colorCO2 = Color.Lerp(MinColor, MaxColor, (float)CO2.value / MaxVal);
        CO2.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = colorCO2;

    }

    private void OnGUI()
    {
        if (OpenWindow)
        {
            Time.timeScale = 0;
            GameOver();

        }
    }

}
