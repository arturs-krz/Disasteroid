using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SynchronizeResources : MonoBehaviour
{
    public Slider popSlider;

    private float nextUpdateTime;
    private float updateRate;

    private float currentCO2;
    private float currentMoney;
    private long currentPopulation;

    private CO2Manager CO2Manage;
    private MoneyManager MoneyManage;
    private PopVegManager popVegManager;

    PhotonView photonView;

    void Start()
    {
        updateRate = 0.5f;
        nextUpdateTime = updateRate;

        CO2Manage = GetComponent<CO2Manager>();
        MoneyManage = GetComponent<MoneyManager>();
        popVegManager = GetComponent<PopVegManager>();

        photonView = PhotonView.Get(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextUpdateTime)
        {
            currentCO2 = CO2Manage.currentCO2;
            currentMoney = MoneyManage.currentMoney;
            currentPopulation = PopVegManager.totalPop;

            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("Synchronize", RpcTarget.Others, currentCO2, currentMoney, currentPopulation);

                // Set timer
                nextUpdateTime += updateRate;
            }
        }
    }

    [PunRPC]
    void Synchronize(float CO2, float Money, long Population)
    {
        NetworkDebugger.Log("Getting resources from server");
        CO2Manager CO2ManageSynch = GetComponent<CO2Manager>();
        MoneyManager MoneyManageSynch = GetComponent<MoneyManager>();

        CO2ManageSynch.currentCO2 = CO2;
        MoneyManageSynch.currentMoney = Money;
        PopVegManager.totalPop = Population;

        //NetworkDebugger.Log("CO2 value on other device: " + CO2);
        //NetworkDebugger.Log("CO2 value on other device: " + CO2ManageSynch.currentCO2);
        //NetworkDebugger.Log("Money value on other device: " + Money);
        //NetworkDebugger.Log("Money value on other device: " + MoneyManageSynch.currentMoney);
        //NetworkDebugger.Log("Population value on other device: " + Population);
        //NetworkDebugger.Log("Population PopVegManager value on other device: " + PopVegManager.totalPop);

        //NetworkDebugger.Log("Population UI value on other device: " + popSlider.value);
        //NetworkDebugger.Log("Population UI maxValue on other device: " + popSlider.maxValue);


    }
}
