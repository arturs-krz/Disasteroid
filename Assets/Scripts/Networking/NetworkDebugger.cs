using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkDebugger : MonoBehaviourPun
{
    private static NetworkDebugger _instance;
    //public static NetworkDebugger Instance { get { return _instance; } }

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

    public static void Log(object logMsg)
    {
        if (PhotonGameLobby.Instance.connected)
        {
            _instance.photonView.RPC("LogFromNetwork", RpcTarget.MasterClient, logMsg);
        }
    }



    [PunRPC]
    void LogFromNetwork(object logObj)
    {
        Debug.Log(logObj);
    }
}
