using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EarthRotationSync : MonoBehaviourPun, IPunObservable
{
    private GameObject earthInstance;
    private Rigidbody earthRb;

    private float lastSync = 0f;

    void Update()
    {
        if (earthInstance == null)
        {
            GameObject earth = GameObject.FindWithTag("Earth");
            if (earth != null)
            {
                FeedEarthInstance(earth);
                lastSync = 0f;
            }
        }
    }
    public void FeedEarthInstance(GameObject earthInstance)
    {
        this.earthInstance = earthInstance;
        this.earthRb = this.earthInstance.GetComponent<Rigidbody>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (earthInstance != null)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(earthRb.rotation);
            }
            else
            {
                Quaternion lastRotation = (Quaternion)stream.ReceiveNext();
                float currentTime = Time.time;
                if (currentTime - lastSync > 1f)
                {
                    // float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                    // Should compensate for lag here
                    // Need to somehow calculate the resulting quaternion by adding 
                    // the angular velocity * lag

                    earthRb.rotation = lastRotation;
                    lastSync = currentTime;
                }
            }

        }
    }
}