using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EarthRotationSync : MonoBehaviourPun, IPunObservable
{
    private GameObject earthInstance;
    private Rigidbody earthRb;

    public Text status;
    // private float lastSync = 0f;

    void Start()
    {
        status = GameObject.Find("Status").GetComponent<Text>();
        status.text = "WORKING";
    }
    public void FeedEarthInstance(GameObject earthInstance)
    {
        this.earthInstance = earthInstance;
        this.earthRb = this.earthInstance.GetComponent<Rigidbody>();

        this.earthRb.angularVelocity = new Vector3(0,0,0);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (earthInstance != null)
        {
            // On master client send the earth rotation updates
            if (stream.IsWriting)
            {
                status.text = "SENDING";
                stream.SendNext(earthInstance.transform.rotation);
            }
            else
            {
                status.text = "UPDATED @ " + Time.time;
                // Sync the rotation on other clients
                //Quaternion lastRotation = (Quaternion)stream.ReceiveNext();
                //earthInstance.transform.localRotation = lastRotation;
                
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