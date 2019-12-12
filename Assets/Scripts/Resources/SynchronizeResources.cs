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
    private long initialPopulation;

    private CO2Manager CO2Manage;
    private MoneyManager MoneyManage;
    private PopVegManager popVegManager;

    PhotonView photonView;

    void Start()
    {
        // Set variables for update rate
        updateRate = 0.5f;
        nextUpdateTime = updateRate;

        // Set resource variables
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
            // Start network-wide call for all clients to synchronize resource levels
            if (PhotonNetwork.IsMasterClient && PhotonGameLobby.Instance.connected)
            {
                currentCO2 = CO2Manage.currentCO2;
                currentMoney = MoneyManage.currentMoney;
                currentPopulation = PopVegManager.totalPop;
                initialPopulation = PopVegManager.initialPop;
                photonView.RPC("Synchronize", RpcTarget.Others, currentCO2, currentMoney, currentPopulation, initialPopulation);

                // Set timer
                nextUpdateTime += updateRate;
            }
        }
    }

    // Method for all clients to synchronize resources according to Master client
    [PunRPC]
    void Synchronize(float CO2, float money, long population, long initialPopulation)
    {
        NetworkDebugger.Log("Getting resources from server");
        CO2Manager CO2ManageSynch = GetComponent<CO2Manager>();
        MoneyManager MoneyManageSynch = GetComponent<MoneyManager>();

        CO2ManageSynch.currentCO2 = CO2;
        MoneyManageSynch.currentMoney = money;
        PopVegManager.totalPop = population;
        PopVegManager.initialPop = initialPopulation;
    }
}
