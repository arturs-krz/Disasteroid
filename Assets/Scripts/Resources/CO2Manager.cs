using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CO2Manager : MonoBehaviour
{
    public Slider CO2Slider;

    [HideInInspector]
    public float maxCO2;
    public float currentCO2;
    public float criticalValue;

    // Start is called before the first frame update
    void Start()
    {
        maxCO2 = 1000;
        currentCO2 = 200;
        criticalValue = Convert.ToSingle(maxCO2 * 0.8);

        CO2Slider.maxValue = maxCO2;
        CO2Slider.value = currentCO2;
    }

    // Update is called once per frame
    void Update()
    {
        //Update CO2 value each frame
        CO2Slider.value = currentCO2;
    }
}
