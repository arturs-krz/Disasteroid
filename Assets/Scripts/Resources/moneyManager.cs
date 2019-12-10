using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public Text textUI;
    
    public long maxPopulation;
    public long currentPopulation;
    public float populationRatio;

    public int updateRate;
    public float increaseMoney;
    public float currentMoney;

    // Start is called before the first frame update
    void Start()
    {
        maxPopulation = PopVegManager.totalPop;
        currentMoney = 10000000000;
        updateRate = 1000000;

        MoneyToText(currentMoney, textUI);
    }

    // Update is called once per frame
    void Update()
    {
        //InvokeRepeating("UpdateMoney", 0f, 0.5f);
    }

    void UpdateMoney()
    {
        //Update currentMoney based on populationRatio and updateRate
        currentPopulation = PopVegManager.totalPop;
        populationRatio = currentPopulation / maxPopulation;
        increaseMoney = populationRatio * updateRate * Time.deltaTime;
        currentMoney += increaseMoney;

        //Call to method to update the money UI 
        MoneyToText(currentMoney, textUI);
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
