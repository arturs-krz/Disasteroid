using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombThrower : MonoBehaviourPun
{
    int bombCost = 50000;
    Vector3 localPosition ;

    public void BombThrowing()
    {
        NetworkDebugger.Log("Clikcing on the UI");
        // positioning
        localPosition = ARController.Instance.earthMarker.transform.InverseTransformPoint(Camera.main.transform.position);
        photonView.RPC("SpawnBomb", RpcTarget.MasterClient, localPosition);
        SpawnBomb(localPosition);
    }

    [PunRPC]
    void SpawnBomb(Vector3 position)
    {
        NetworkDebugger.Log("Spwan Bomb __ PunRPC");
        GameObject resourceManager = GameObject.FindGameObjectWithTag("ResourceManager");
        moneyManager moneyManage = resourceManager.GetComponent<moneyManager>();

        //comment this when the other thingy is working
        moneyManage.currentMoney -= bombCost;
        PhotonNetwork.Instantiate("AR-bomb", localPosition, Quaternion.identity);

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
