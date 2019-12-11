using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public Text textUI;

    [HideInInspector]
    public long maxPopulation;
    [HideInInspector]
    public long currentPopulation;
    [HideInInspector]
    public float populationRatio;

    [HideInInspector]
    public int updateRate;
    [HideInInspector]
    public float increaseMoney;
    [HideInInspector]
    public float currentMoney;

    float currentPopulationInt;
    float maxPopulationInt;

    private float nextUpdateTime;
    private float updateRateTime;

    // Start is called before the first frame update
    void Start()
    {
        maxPopulation = 6167860246/10000000;
        currentMoney = 10000000000;
        updateRate = 1000000000;

        MoneyToText(currentMoney, textUI);

        updateRateTime = 0.5f;
        nextUpdateTime = updateRateTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextUpdateTime)
        {
            //Update currentMoney based on populationRatio and updateRate
            currentPopulation = PopVegManager.totalPop/10000000;

            currentPopulationInt = 1;

            //NetworkDebugger.Log("currentPopulation as int is: " + currentPopulationInt);

            maxPopulationInt = 2;

            //NetworkDebugger.Log("maxPopulation as int is: " + maxPopulationInt);
            
            populationRatio = currentPopulationInt / maxPopulationInt;

            //NetworkDebugger.Log("PopulationRatio is: " + populationRatio);

            increaseMoney = populationRatio * updateRate * Time.deltaTime;
            currentMoney += increaseMoney;

            //NetworkDebugger.Log("increaseMoney is: " + increaseMoney);
            //NetworkDebugger.Log("currentMoney is: " + currentMoney);

            //Call to method to update the money UI 
            MoneyToText(currentMoney, textUI);

            // Set timer
            nextUpdateTime += updateRateTime;
        }
    }

    void MoneyToText(float currentMoney, Text textUI)
    {
        float previewMoney;

        //update money UI with correct format
        if ((currentMoney / 1000000000) > 1)
        {
            previewMoney = currentMoney / 1000000000;
            textUI.text = Math.Round(previewMoney, 1) + "B";
        }
        else if ((currentMoney / 1000000) > 1)
        {
            previewMoney = currentMoney / 1000000;
            textUI.text = Math.Round(previewMoney, 1) + "M";
        }
        else if ((currentMoney / 1000) > 1)
        {
            previewMoney = currentMoney / 1000;
            textUI.text = Math.Round(previewMoney, 1) + "K";
        }
        else
        {
            previewMoney = currentMoney;
            textUI.text = previewMoney.ToString("0");
        }
    }
}
