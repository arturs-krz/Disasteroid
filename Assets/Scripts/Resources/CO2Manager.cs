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
    [HideInInspector]
    public float currentCO2;
    [HideInInspector]
    public float criticalCO2;
    [HideInInspector]
    public float impactCO2;

    private float nextUpdateTime;
    private float updateRate;

    void Awake()
    {
        maxCO2 = 100;
        currentCO2 = 20;
        criticalCO2 = Convert.ToSingle(maxCO2 * 0.75);
        impactCO2 = 2f;   
    }
    
    // Start is called before the first frame update
    void Start()
    {
        CO2Slider.maxValue = maxCO2;
        CO2Slider.value = currentCO2;

        // Set timer variables for update call
        updateRate = 0.5f;
        nextUpdateTime = updateRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextUpdateTime)
        {
            //Update CO2 UI at set times
            CO2Slider.value = currentCO2;

            // Set timer
            nextUpdateTime += updateRate;

            Shader.SetGlobalFloat("_CO2", currentCO2/ maxCO2);
        }
    }
}
