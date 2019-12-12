using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Buy : MonoBehaviour
{
    private int bombCost;

    void Start()
    {
        bombCost = 50000000;

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("buyStuff", RpcTarget.All, bombCost);
        }
    }

    [PunRPC]
    void buyStuff(int costs)
    {
        GameObject resourceManager = GameObject.FindGameObjectWithTag("ResourceManager");
        MoneyManager moneyManage = resourceManager.GetComponent<MoneyManager>();

        moneyManage.currentMoney -= costs;
    }


}
