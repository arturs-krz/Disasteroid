<<<<<<< HEAD
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

    //private bool OpenWindow;
    int MaxVal = 100;

    // Start is called before the first frame update
    void Start()
    {
        //OpenWindow = false;
    }

    public void BacktoMain()
    {
        SceneManager.LoadScene("GameStart");
    }

    //public void ExitGame()
    //{
    //    Application.Quit();
    //}

    //public void Continue()
    //{
    //    //OpenWindow = false;
    //    gameover.SetActive(false);
    //    Time.timeScale = 1;
    //}

    //public void GameOver()
    //{
    //    gameover.SetActive(true);
    //    //Application.Quit(); 
    //}

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.Escape))
        //{
        //    Time.timeScale = 0;
        //    GameOver();
        //}

        Color colorPD = Color.Lerp(MinColor, MaxColor, (float)PD.value / MaxVal);
        PD.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = colorPD;

        Color colorCO2 = Color.Lerp(MinColor, MaxColor, (float)CO2.value / MaxVal);
        CO2.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = colorCO2;

    }
}
=======
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
        //WL.value += Time.deltaTime * CO2.value / 10;
        PD.value += Time.deltaTime * CO2.value / 20;
        Color colorPD = Color.Lerp(MinColor, MaxColor, (float)PD.value / MaxVal);
        populationSliderFill.color = colorPD;

        CO2.value += Time.deltaTime * (PD.maxValue - PD.value) * 0.001f;
        Color colorCO2 = Color.Lerp(MinColor, MaxColor, (float)CO2.value / MaxVal);
        CO2SliderFill.color = colorCO2;

        moneyDisplay.text = moneyValue.ToString();
        // if (PD.value >= PD.maxValue || WL.value >= WL.maxValue || CO2.value >= CO2.maxValue)
        // {
        //     GameOver();
        // }

    }
}
>>>>>>> 1bc36b818b1ba132fe172cc219b5cf14b19d9d8e
