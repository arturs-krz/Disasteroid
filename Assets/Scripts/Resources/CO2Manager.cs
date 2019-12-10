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
    public float criticalCO2;
    public float impactCO2;

    void Awake()
    {
        maxCO2 = 100;
        currentCO2 = 20;
        criticalCO2 = Convert.ToSingle(maxCO2 * 0.8);
        impactCO2 = 4;   
    }
    
    // Start is called before the first frame update
    void Start()
    {
        CO2Slider.maxValue = maxCO2;
        CO2Slider.value = currentCO2;
    }

    // Update is called once per frame
    void Update()
    {
        //InvokeRepeating("UpdateUI", 0f, 0.5f); 
        
    }

    void UpdateUI()
    {
        //Update CO2 UI at set times
        CO2Slider.value = currentCO2;
    }
}
