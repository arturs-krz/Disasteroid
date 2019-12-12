using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombScript : MonoBehaviourPun
{
    GameObject asteroid;


    public GameObject explosionEffect;
    public GameObject propellers;

    // speed in which the bomb goes towards the target
    public float speed = 5f;
    public float radius = 50f;
    public float force = 500f;

    private int bombCost;

    public float delay = 3f;
    float countdown;

    bool startcountdown = false;
    bool hasExploded = false;
    bool targetfound = false;

    Vector3 position = new Vector3(0f, 0f, 0f);
    Vector3 aposition = new Vector3(0f, 0f, 0f);

    public void Start()
    {
        Vector3 camPos = Camera.main.gameObject.transform.position;
        bombCost = 5000;

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     PhotonView photonView = PhotonView.Get(this);
        //     photonView.RPC("buyStuff", RpcTarget.All, bombCost);
        // }

        countdown = delay;
        // display on screen "searching"
        asteroid = SeekClosest();

        // On all phones
        if (!PhotonNetwork.IsMasterClient)
        {
            Vector3 localPosition = transform.position;
            transform.SetParent(ARController.Instance.earthMarker.transform);
            transform.localPosition = localPosition;
            position = transform.position;

            NetworkDebugger.Log("Bomb spawned on client");
        }
    }

    void Update()
    {
        if(startcountdown) {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {

                if (countdown <= 0f && hasExploded == false)
                {
                    NetworkDebugger.Log("Ready to boom boom baby");
                    Explode();
                    hasExploded = true;
                }
            }
        }

        if (targetfound == true)
        {
            aposition = asteroid.transform.position;
            //gameObject.transform.position = Vector3.MoveTowards(position, aposition, speed * Time.deltaTime);
            position = gameObject.transform.position;

            //gameObject.transform.LookAt(aposition);
            
            //NetworkDebugger.Log("ast_pos: " + aposition + " // rocket_pos: " + position);

            //StartPropellers();
            if (Vector3.Distance(aposition, position) < 1f || asteroid == null)
            {
                if (startcountdown == false)
                {
                    startcountdown = true;
                }
            }
        }
    }

    //void StartPropellers()
    //{
    //    Instantiate(propellers, transform.position, transform.rotation);
    //}

    GameObject SeekClosest()
    {
        float d = 10000000;
        foreach (GameObject nearbyAsteroid in AsteroidSpawner.Instance.asteroids)
        {
            aposition = nearbyAsteroid.transform.position;
            float d1 = Vector3.Distance(position, aposition);
            if (d1 < d)
            {
                d = d1;
                asteroid = nearbyAsteroid;
            }
        }
        targetfound = true;
        return asteroid;
    }

    void Explode()
    {
        //Destroy(propellers);
        NetworkDebugger.Log("BOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM");
        
        GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (GameObject nearbyAsteroid in AsteroidSpawner.Instance.asteroids)
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
        
        // remove bomb
        // FOR NOW, ONLY LOCALLY
        Destroy(explosion, 2f);
        PhotonNetwork.Destroy(gameObject);
    }
}

