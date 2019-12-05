using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    readonly GameObject Bomb;
    readonly GameObject Asteroid;

    GameObject target;

    public GameObject explosionEffect;
    public GameObject propellers;

    // speed in which the bomb goes towards the target
    public float speed = 50f;
    public float radius = 5f;
    public float force = 500f;
    public float delay = 3f;

    float countdown;

    bool startcountdown = false;
    bool hasExploded = false;
    bool targetFound = true;
    bool targetfound = false;
    Vector3 pos = new Vector3(0f, 0f, 0f);

    public void Start()
    {
        //public GameObject bomb = Instantiate(Bomb, transform.position, transform.rotation);
        //public GameObject asteroid = Instantiate(Asteroid, transform.position, transform.rotation);
        // should i get this as camera position??
        pos = Camera.main.gameObject.transform.position;

        countdown = delay;

        // display on screen "searching"
        GameObject target = SeekClosest();
    }

    void Update()
    {
        if(startcountdown) {
            countdown -= Time.deltaTime;
        }

        if (targetfound)
        {
            Intercept(target);
        }

        if (countdown < 0 && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Intercept(GameObject target)
    {
        Vector3 apos = target.transform.position;
        Vector3 direction = apos - pos;

        transform.SetPositionAndRotation(pos + (speed*direction) , transform.rotation);
        // propellers
        StartPropellers();
        if (Vector3.Distance(apos, pos) < 10f) {
            startcountdown = true;
        }
        
    }

    void StartPropellers()
    {
        Instantiate(propellers, transform.position, transform.rotation);
    }

    GameObject SeekClosest()
    {
        GameObject asteroid = null;

        float d = 10000000;
        foreach(GameObject nearbyAsteroid in AsteroidSpawner.Instance.asteroids)
        {
            Vector3 apos = nearbyAsteroid.transform.position;
            float d1 = Vector3.Distance(pos, apos);
            if (d1 < d)
            {
                d = d1;
                asteroid = nearbyAsteroid;
                targetfound = true;
            }
        }
            return asteroid;
    }

    void Explode()
    {
        //Debug.Log("Boom");
        // show effect
        Destroy(propellers);
         
        Instantiate(explosionEffect, transform.position, transform.rotation);

        foreach(GameObject nearbyAsteroid in AsteroidSpawner.Instance.asteroids)
        {
            // add force -> move
            Rigidbody rb = nearbyAsteroid.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
            // damage
        }
        // remove bomb
        Destroy(gameObject);
    }

}

