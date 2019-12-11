using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SatelliteSpawner : MonoBehaviourPun
{
    private static SatelliteSpawner _instance;

    public static bool isSatelliteActive = false;

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
        
    }

    public static void SpawnSatellite()
    {
        if (!isSatelliteActive)
        {
            isSatelliteActive = true;
            Transform cameraTransform = ARController.Instance.FirstPersonCamera.transform;
            Vector3 spawnPosition = cameraTransform.position + (cameraTransform.forward * 0.2f);
            // Vector3 spawnOrientation = (Quaternion.LookRotation(cameraTransform.forward, Vector3.up) * Quaternion.Euler(0, 90f, 0)).eulerAngles;

            NetworkDebugger.Log("Requesting satellite spawn");
            Vector3 localPosition = ARController.Instance.earthMarker.transform.InverseTransformPoint(spawnPosition);
            //Vector3 localOrientation = ARController.Instance.earthMarker.transform.InverseTransformDirection(-cameraTransform.forward);
            Vector3 localOrientation = new Vector3(0,0,0);
            
            _instance.photonView.RPC("SpawnSatelliteOnServer", RpcTarget.MasterClient, localPosition, localOrientation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void SpawnSatelliteOnServer(Vector3 localPosition, Vector3 localOrientation)
    {
        if (PhotonNetwork.IsMasterClient && !isSatelliteActive)
        {
            // Check and deduce funds

            Debug.Log("Instantiating satellite on the server");
            PhotonNetwork.Instantiate("AR-Satellite", localPosition, Quaternion.Euler(localOrientation));
            isSatelliteActive = true;
        }
    }
}
