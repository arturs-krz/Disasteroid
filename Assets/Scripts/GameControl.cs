using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public Slider PD;
    public Slider CO2;
    int moneyValue;
    double bombPrice = 10e5;
    public Text userDisplay;
    public Text moneyDisplay;
    public GameObject gameover;

    //color change for sliders
    public int MaxVal = 100;
    public Color MaxColor;
    public Color MinColor;


    private Image populationSliderFill;
    private Image CO2SliderFill;

    // Start is called before the first frame update
    void Start()
    {
        //gameover.SetActive(false);
        //if (PlayerPrefs.GetString("Username") == "")
        //{
        //    PlayerPrefs.SetString("Username", "John Doe");
        //}

        //userDisplay.text = PlayerPrefs.GetString("Username").ToString() + " is playing!";
        //userDisplay.text = PlayerPrefs.GetString("Username").ToString();
  
        moneyValue = (int)(PD.value * 10e8);
        moneyDisplay.text = "$ " + moneyValue.ToString("n0");
        PD.minValue = 0f;
        PD.maxValue = MaxVal;
        CO2.minValue = 0f;
        CO2.maxValue = MaxVal;
        CO2.value = CO2.maxValue/2;
        PD.value = PD.maxValue/2;
    }

    public void BombAttack()
    {
        PD.value -= PD.maxValue / 20;
        CO2.value += MaxVal / 10;
        moneyValue = (int)(moneyValue + PD.value * 10e6  - bombPrice);
        moneyDisplay.text = "$ " + moneyValue.ToString("n0");
    }

    public void SatelitteScan()
    {

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
        //WL.value += Time.deltaTime * CO2.value / 10;
        PD.value += Time.deltaTime * CO2.value / 20;
        Color colorPD = Color.Lerp(MinColor, MaxColor, (float)PD.value / MaxVal);
        PD.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = colorPD;

        CO2.value -= Time.deltaTime * (PD.maxValue - PD.value) * 0.001f;
        Color colorCO2 = Color.Lerp(MinColor, MaxColor, (float)CO2.value / MaxVal);
        CO2.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = colorCO2;

        moneyDisplay.text = "$ " + moneyValue.ToString("n0");
        // if (PD.value >= PD.maxValue || WL.value >= WL.maxValue || CO2.value >= CO2.maxValue)
        // {
        //     GameOver();
        // }

    }
}
