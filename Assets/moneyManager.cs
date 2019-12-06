using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moneyManager : MonoBehaviour
{
    private int maxPopulation;
    private int currentPopulation;
    private float populationRatio;

    public int updateRate;
    private float updateMoney;
    public float currentMoney;
    private float previewMoney;

    public Text textUI;

    // Start is called before the first frame update
    void Start()
    {
        maxPopulation = 1; //Reference proper script
        currentMoney = 10000000000;
        updateRate = 10000;

        if ((currentMoney / 1000000000) > 1)
        {
            previewMoney = currentMoney / 1000000000;
            textUI.text = Math.Round(previewMoney, 1) + "B";
        } else if ((currentMoney / 1000000) > 1)
        {
            previewMoney = currentMoney / 1000000;
            textUI.text = Math.Round(previewMoney, 1) + "M";
        } else if ((currentMoney / 1000) > 1)
        {
            previewMoney = currentMoney / 1000;
            textUI.text = Math.Round(previewMoney, 1) + "K";
        } else
        {
            previewMoney = currentMoney;
            textUI.text = previewMoney.ToString("0");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        currentPopulation = 1; //Reference proper script
        populationRatio = currentPopulation / maxPopulation;
        updateMoney = populationRatio * updateRate;
        currentMoney += updateMoney;

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
