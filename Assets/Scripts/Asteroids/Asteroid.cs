using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Asteroid : MonoBehaviourPun, IPunObservable
{

    private Rigidbody rb;

    Vector3 lastPosition;
    Quaternion lastRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // This will be true for the master client who is spawning the asteroids
        if (stream.IsWriting)
        {
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);

            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // sync on the other clients
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();

            //lastPosition = (Vector3)stream.ReceiveNext();
            //lastRotation = (Quaternion)stream.ReceiveNext();

            //lastPosition = lastPosition + ARController.Instance.earthInstance.transform.position;

            rb.position = (Vector3)stream.ReceiveNext() + ARController.Instance.earthInstance.transform.position;
            rb.rotation = (Quaternion)stream.ReceiveNext();


            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            rb.position = rb.position + (rb.velocity * lag); // Compensate position
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Vector3 pos = ARController.Instance.earthInstance.transform.position - transform.position;
            rb.AddForce(Time.fixedDeltaTime * 1f * pos.normalized / pos.sqrMagnitude);

            // Destroy if it gets away
            if (pos.magnitude > 2.2f)
            {
                PhotonNetwork.Destroy(gameObject);
                AsteroidSpawner.numberOfAsteroids -= 1;
            }
        }
        //else
        //{
        //    rb.position = Vector3.MoveTowards(rb.position, lastPosition, Time.fixedDeltaTime * rb.velocity.magnitude);
        //    rb.rotation = Quaternion.RotateTowards(rb.rotation, lastRotation, Time.fixedDeltaTime * rb.angularVelocity.magnitude);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            //Debug.Log(other.gameObject.tag);    
            PhotonNetwork.Destroy(gameObject);

            AsteroidSpawner.numberOfAsteroids -= 1;
            GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
            GameControl gameControl = gameController.GetComponent<GameControl>();
            gameControl.PD.value -= gameControl.PD.maxValue / 30;
            gameControl.CO2.value += gameControl.CO2.maxValue / 60;
        }

    }
}
