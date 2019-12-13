using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombScript : MonoBehaviourPun, IPunObservable
{


    public GameObject explosionEffect;
    //public GameObject propellers;
    private GameObject targetAsteroid;
    private Rigidbody targetRb;

    private ParticleSystem propulsion;

    // speed in which the bomb goes towards the target
    private float speed = 0.75f;
    private float radius = 1.5f;
    private float force = 0.15f;


    private float range = 1f;
    private float delay = 2f;
    private float countdown;

    private Rigidbody rb;

    bool startcountdown = false;
    bool hasExploded = false;
    bool targetFound = false;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        propulsion = transform.GetChild(0).GetComponent<ParticleSystem>();
 
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     PhotonView photonView = PhotonView.Get(this);
        //     photonView.RPC("buyStuff", RpcTarget.All, bombCost);
        // }

        countdown = delay;

        // On all phones
        if (!photonView.IsMine)
        {
            Vector3 localPosition = transform.position;
            Quaternion localRotation = transform.rotation;

            transform.SetParent(ARController.Instance.earthMarker.transform);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;   
        }
        SeekClosest();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && hasExploded == false)
            {
                Explode();
            }
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (targetAsteroid != null)
            {
                Vector3 direction = targetAsteroid.transform.position - transform.position;
                transform.LookAt(targetAsteroid.transform.position);
                rb.velocity = direction.normalized * speed;

                if (direction.magnitude < 0.05f)
                {
                    Explode();
                }
            }
        }
        else
        {
            if (targetAsteroid != null)
            {
                Vector3 direction = (targetAsteroid.transform.position + (targetRb.velocity * Time.fixedDeltaTime)) - transform.position;
                transform.LookAt(targetAsteroid.transform.position);
                transform.localPosition += transform.InverseTransformDirection(direction.normalized) * speed * Time.fixedDeltaTime;
                //rb.velocity = direction.normalized * speed;
            }
        }
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.velocity);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            rb.velocity = ARController.Instance.earthMarker.transform.InverseTransformDirection((Vector3)stream.ReceiveNext());
            transform.localPosition = (Vector3)stream.ReceiveNext();
            transform.localRotation = (Quaternion)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            transform.localPosition = transform.localPosition + ARController.Instance.earthMarker.transform.InverseTransformDirection(rb.velocity * lag);
        }
    }

    //void StartPropellers()
    //{
    //    Instantiate(propellers, transform.position, transform.rotation);
    //}

    void SeekClosest()
    {
        float d = range;
        foreach (GameObject nearbyAsteroid in AsteroidSpawner.asteroidInstances)
        {
            float d1 = Vector3.Distance(transform.position, nearbyAsteroid.transform.position);
            if (d1 <= d)
            {
                d = d1;
                targetAsteroid = nearbyAsteroid;
                targetRb = targetAsteroid.GetComponent<Rigidbody>();
                targetFound = true;

                propulsion.Play();
            }
        }

        if (targetFound == false)
        {
            rb.velocity = transform.forward * speed * 0.1f;
        }
    }

    void Explode()
    {   
        if (photonView.IsMine)
        {
            foreach (GameObject nearbyAsteroid in AsteroidSpawner.asteroidInstances)
            {
                if ((nearbyAsteroid.transform.position - transform.position).magnitude < radius)
                {
                    nearbyAsteroid.GetComponent<Asteroid>().AffectByExplosion(force, transform.position, radius);
                }
            }
            PhotonNetwork.Instantiate("DustExplosion", transform.position, transform.rotation);
            PhotonNetwork.Destroy(gameObject);
        }
        hasExploded = true;
        
    }
}

