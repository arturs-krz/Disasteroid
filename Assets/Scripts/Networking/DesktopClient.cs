using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class DesktopClient : MonoBehaviour
{
    public GameObject resourceManager;
    private CO2Manager CO2manager;
    private MoneyManager moneyManager;

    public GameObject gameOverScreen;

    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log("We're not on Android! Could it be the PC? :o");

            // Disable the AR specific GameObjects
            GameObject ARDevice = GameObject.Find("ARCore Device");
            ARDevice.SetActive(false);

            //GameObject ARController = GameObject.Find("Diasteroid AR Controller");
            //ARController.SetActive(false);

            CO2manager = resourceManager.GetComponent<CO2Manager>();
            moneyManager = resourceManager.GetComponent<MoneyManager>();
        }
        else
        {
            // Otherwise disable the desktop camera
            GameObject desktopCamera = GameObject.Find("DesktopCamera");
            desktopCamera.SetActive(false);

            // And this component for that matter
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Reset all the shit!
            foreach(GameObject asteroid in AsteroidSpawner.asteroidInstances)
            {
                PhotonNetwork.Destroy(asteroid);
            }
            AsteroidSpawner.numberOfAsteroids = 0;
            AsteroidSpawner.asteroidInstances = new List<GameObject>();

            CO2manager.currentCO2 = 20;
            PopVegManager.totalPop = PopVegManager.initialPop;
            moneyManager.currentMoney = 10000000000;
            PopVegManager.gameOver = false;
            gameOverScreen.SetActive(false);
        }
    }
}
