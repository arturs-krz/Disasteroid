using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CO2Manager : MonoBehaviour
{
    public Slider CO2Slider;

    public float maxCO2;
    public float currentCO2;


    // Start is called before the first frame update
    void Start()
    {
        maxCO2 = 100;
        currentCO2 = 20;

        CO2Slider.maxValue = maxCO2;
        CO2Slider.value = currentCO2;
    }

    // Update is called once per frame
    void Update()
    {
        CO2Slider.value = currentCO2;
    }
}
