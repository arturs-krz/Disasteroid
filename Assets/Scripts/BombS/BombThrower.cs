using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombThrower : MonoBehaviourPun
{
    int bombCost = 50000;

    public void BombThrowing()
    {
        NetworkDebugger.Log("Clicking on the UI");
        // positioning
        Vector3 spawnPoint = ARController.Instance.FirstPersonCamera.transform.position + (ARController.Instance.FirstPersonCamera.transform.forward * 0.1f);
        Vector3 localPosition = ARController.Instance.earthMarker.transform.InverseTransformPoint(spawnPoint);
        Vector3 localOrientation = ARController.Instance.earthMarker.transform.InverseTransformDirection(ARController.Instance.FirstPersonCamera.transform.forward);
        photonView.RPC("SpawnBomb", RpcTarget.MasterClient, localPosition, localOrientation);
    }

    [PunRPC]
    void SpawnBomb(Vector3 position, Vector3 localOrientation)
    {
        Debug.Log("Received bomb spawn request");
        // GameObject resourceManager = GameObject.FindGameObjectWithTag("ResourceManager");
        // moneyManager moneyManage = resourceManager.GetComponent<moneyManager>();

        //comment this when the other thingy is working
        // moneyManage.currentMoney -= bombCost;
        PhotonNetwork.Instantiate("AR-bomb", position, Quaternion.LookRotation(localOrientation));

        //if(moneyManage.currentMoney > costs)
        //{
        //    moneyManage.currentMoney -= costs;
        //    PhotonNetwork.Instantiate("AR-Bomb", localPosition, Quaternion.identity);
        //}
        //else
        //{
        //    // write: not enough moneyyyy
        //}
    }
}
