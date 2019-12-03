using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Asteroid : MonoBehaviourPun, IPunObservable
{

    private Rigidbody rb;

    /// <summary>
    /// The initial scaling of the asteroid when it's spawned.
    /// Used to calculate the forced perspective afterwards
    /// </summary>
    private Vector3 baseScale;

    /// <summary>
    /// Relative Earth position. Vector from the asteroid to Earth.
    /// </summary>
    private Vector3 earthPos;

    /// <summary>
    /// Initial relative Earth position.
    /// Used to calculate the forced perspective.
    /// </summary>
    private Vector3 initialEarthPos;

    //private Vector3 lastPosition;
    //private Quaternion lastRotation;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            // Get the position of the Earth instance and calculate the position vector against it.
            earthPos = ARController.Instance.earthInstance.transform.position - transform.position;
            initialEarthPos = earthPos;

            baseScale = transform.localScale;
        }
        else
        {
            transform.SetParent(ARController.Instance.earthMarker.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Manually specify the initial scale of the asteroid.
    /// </summary>
    /// <param name="scale">transform.localScale</param>
    public void SetInitialScale(Vector3 scale)
    {
        baseScale = scale;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // This will be true for the master client who is spawning the asteroids
        if (stream.IsWriting)
        {
            // Send all the physics properties from the master simulation.
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
            stream.SendNext(transform.localScale);

            stream.SendNext(transform.localPosition);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // Sync on the other clients
            
            rb.velocity = ARController.Instance.earthMarker.transform.TransformDirection((Vector3)stream.ReceiveNext());
            //rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();

            transform.localScale = (Vector3)stream.ReceiveNext();

            //lastPosition = (Vector3)stream.ReceiveNext();
            //lastRotation = (Quaternion)stream.ReceiveNext();

            //lastPosition = lastPosition + ARController.Instance.earthInstance.transform.position;

            transform.localPosition = (Vector3)stream.ReceiveNext();
            rb.rotation = (Quaternion)stream.ReceiveNext();

            // Calculate the time delta (lag) between current server time and when the latest update was sent
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            // Compensate position
            transform.localPosition = transform.localPosition + (rb.velocity * lag);
        }
    }

    void FixedUpdate()
    {
        // On the master client (IsMine true for the owner of object)
        if (photonView.IsMine)
        {
            earthPos = ARController.Instance.earthInstance.transform.position - transform.position;
            rb.AddForce(Time.fixedDeltaTime * 1f * earthPos.normalized / earthPos.sqrMagnitude);

            // Destroy if it gets away
            if (earthPos.magnitude > 2.2f)
            {
                PhotonNetwork.Destroy(gameObject);
                AsteroidSpawner.numberOfAsteroids -= 1;
            }

            // Forced perspective. Make it appear slightly smaller if it's close to earth.
            transform.localScale = baseScale * Mathf.Clamp((earthPos.magnitude / initialEarthPos.magnitude) + 0.5f, 0.5f, 1.0f);
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
