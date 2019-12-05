using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EarthRotationSync : MonoBehaviourPun, IPunObservable
{
    private GameObject earthInstance;
    //private Rigidbody earthRb;

    // private float lastSync = 0f;

    public void FeedEarthInstance(GameObject earth)
    {
       earthInstance = earth;
       //earthRb = earthInstance.GetComponent<Rigidbody>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (earthInstance != null)
        {
            // On master client send the earth rotation updates
            if (stream.IsWriting)
            {
                stream.SendNext(earthInstance.transform.rotation);
            }
            else
            {
                // Sync the rotation on other clients
                Quaternion lastRotation = (Quaternion)stream.ReceiveNext();
                earthInstance.transform.localRotation = lastRotation;

                // float currentTime = Time.time;
                // if (currentTime - lastSync > 1f)
                // {
                //     // float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                //     // Should compensate for lag here
                //     // Need to somehow calculate the resulting quaternion by adding 
                //     // the angular velocity * lag

                //     earthRb.rotation = lastRotation;
                //     lastSync = currentTime;
                // }
            }

        }
    }
}