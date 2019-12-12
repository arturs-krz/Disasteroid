using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombScript : MonoBehaviourPun
{


    public GameObject explosionEffect;
    public GameObject propellers;
    private GameObject targetAsteroid;

    // speed in which the bomb goes towards the target
    public float speed = 2f;
    public float radius = 1f;
    public float force = 0.1f;

    private int bombCost;

    public float range = 1f;
    public float delay = 3f;
    private float countdown;

    private Rigidbody rb;

    bool startcountdown = false;
    bool hasExploded = false;
    bool targetFound = false;

    Vector3 position = new Vector3(0f, 0f, 0f);
    Vector3 aposition = new Vector3(0f, 0f, 0f);

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        bombCost = 5000;

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     PhotonView photonView = PhotonView.Get(this);
        //     photonView.RPC("buyStuff", RpcTarget.All, bombCost);
        // }

        countdown = delay;

        // On all phones
        if (!PhotonNetwork.IsMasterClient)
        {
            Vector3 localPosition = transform.position;
            Quaternion localRotation = transform.rotation;

            transform.SetParent(ARController.Instance.earthMarker.transform);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            position = transform.position;
        }

        SeekClosest();
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && hasExploded == false  && photonView.IsMine)
        {
            Explode();
        }

        if (targetFound == true)
        {
            Vector3 direction = targetAsteroid.transform.position - transform.position;
            transform.LookAt(targetAsteroid.transform.position);
            // rb.AddForce(direction.normalized * 0.5f);
            transform.Translate(direction.normalized * speed * Time.deltaTime);

            if (direction.magnitude < 0.2f && photonView.IsMine)
            {
                Explode();
            }
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
            float d1 = Vector3.Distance(position, nearbyAsteroid.transform.position);
            if (d1 <= d)
            {
                d = d1;
                targetAsteroid = nearbyAsteroid;
                targetFound = true;
            }
        }

        if (targetFound == false)
        {
            rb.angularVelocity = new Vector3(Random.value * 0.2f, Random.value * 0.2f, Random.value * 0.2f);
        }
    }

    void Explode()
    {   
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject nearbyAsteroid in AsteroidSpawner.asteroidInstances)
            {
                // add force -> move
                Rigidbody rb = nearbyAsteroid.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(force, transform.position, radius);
                }
                // damage
            }
            PhotonNetwork.Instantiate("DustExplosion", transform.position, transform.rotation);
            PhotonNetwork.Destroy(gameObject);
        }
        hasExploded = true;
        
    }
}

