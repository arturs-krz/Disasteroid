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
    public float speed = 5f;
    public float radius = 2f;
    public float force = 10f;

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

            NetworkDebugger.Log("Bomb spawned on client");
        }

        SeekClosest();
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && hasExploded == false)
        {
            NetworkDebugger.Log("Ready to boom boom baby");
            Explode();
            hasExploded = true;
        }

        if (targetFound == true)
        {
            Vector3 direction = (targetAsteroid.transform.position - transform.position).normalized;
            transform.LookAt(targetAsteroid.transform.position);
            rb.AddForce(direction * 0.5f);
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
    }

    void Explode()
    {
        //Destroy(propellers);
        NetworkDebugger.Log("BOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM");
        
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);

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
        }
        
        Destroy(explosion, 2f);
        PhotonNetwork.Destroy(gameObject);
    }
}

