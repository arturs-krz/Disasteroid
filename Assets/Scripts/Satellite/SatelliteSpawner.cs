using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SatelliteSpawner : MonoBehaviourPun
{
    private static SatelliteSpawner _instance;

    public static bool isSatelliteActive = false;

    private MoneyManager moneyManager;

    private float startTime;
    private bool showedInfo = false;

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        moneyManager = GameObject.FindWithTag("ResourceManager").GetComponent<MoneyManager>();

        PhotonGameLobby.OnJoinGame += (isMaster) =>
        {
            if (!isMaster)
            {
                startTime = Time.time;
            }
        };
    }

    public static void SpawnSatellite()
    {
        if (!isSatelliteActive && _instance.moneyManager.currentMoney >= 1e9f)
        {
            isSatelliteActive = true;
            Transform cameraTransform = ARController.Instance.FirstPersonCamera.transform;
            Vector3 spawnPosition = cameraTransform.position + (cameraTransform.forward * 0.3f) + (cameraTransform.up * 0.1f);
            // Vector3 spawnOrientation = (Quaternion.LookRotation(cameraTransform.forward, Vector3.up) * Quaternion.Euler(0, 90f, 0)).eulerAngles;

            NetworkDebugger.Log("Requesting satellite spawn");
            Vector3 localPosition = ARController.Instance.earthMarker.transform.InverseTransformPoint(spawnPosition);
            Vector3 localOrientation = ARController.Instance.earthMarker.transform.InverseTransformDirection(cameraTransform.forward);
            // Vector3 localOrientation = new Vector3(0,0,0);
            
            _instance.moneyManager.currentMoney -= 1e9f;
            _instance.photonView.RPC("SpawnSatelliteOnServer", RpcTarget.MasterClient, localPosition, localOrientation);
        }
        else if (_instance.moneyManager.currentMoney < 1e9f)
        {
            UIMessage.ShowMessage("Insufficient funds!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!showedInfo && Time.time - startTime > 10f && !PhotonNetwork.IsMasterClient)
        {
            showedInfo = true;
            UIMessage.ShowMessage("Spawn defensive satellite by scanning the marker!");
        }
    }

    [PunRPC]
    void SpawnSatelliteOnServer(Vector3 localPosition, Vector3 localOrientation)
    {
        if (PhotonNetwork.IsMasterClient && !isSatelliteActive)
        {
            if (moneyManager.currentMoney >= 1e9f)
            {
                moneyManager.currentMoney -= 1e9f;
                Debug.Log("Instantiating satellite on the server");
                PhotonNetwork.Instantiate("AR-Satellite", localPosition, Quaternion.LookRotation(localOrientation));
                isSatelliteActive = true;
            }
        }
    }
}
