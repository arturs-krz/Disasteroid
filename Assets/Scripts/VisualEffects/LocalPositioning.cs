using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LocalPositioning : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // If running on one of the phones
        if (!PhotonNetwork.IsMasterClient)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            transform.SetParent(ARController.Instance.earthMarker.transform);
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
    }

}
