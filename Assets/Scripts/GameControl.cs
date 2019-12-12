using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public Slider PD;
    public Slider CO2;
    int moneyValue;
    double bombPrice = 10e7;
    public Text userDisplay;
    public Text moneyDisplay;
    public GameObject gameover;

    //color change for sliders
    public int MaxVal = 100;
    public Image Fill;  // assign in the editor the "Fill"
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
        moneyDisplay.text = moneyValue.ToString();
        PD.minValue = 0f;
        PD.maxValue = MaxVal;
        CO2.minValue = 0f;
        CO2.maxValue = MaxVal;
        CO2.value = PD.minValue;
        PD.value = PD.minValue;

        populationSliderFill = PD.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        CO2SliderFill = CO2.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
    }

    public void BombAttack()
    {
        PD.value += PD.maxValue / 10;
        CO2.value += 100 / 5;
        moneyValue = (int)(PD.value * 10e8 - bombPrice);
        moneyDisplay.text = moneyValue.ToString();
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
    }
}
