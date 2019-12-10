using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombThrower : MonoBehaviour
{
    bool enoughMoney = true;
    public void BombThrowing()
    {
        if(enoughMoney) {
            NetworkDebugger.Log("Clikcing on the UI");
            // positioning
            Vector3 localPosition = ARController.Instance.earthMarker.transform.InverseTransformPoint(Camera.main.transform.position);
            GameObject bomb = PhotonNetwork.Instantiate("AR-Bomb", localPosition, Quaternion.identity);
        } else {
            //show on screen: "not enough money";
        }

    }
}
