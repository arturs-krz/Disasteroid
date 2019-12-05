using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    public float delay = 3f;
    readonly GameObject Bomb;
    readonly GameObject Asteroid;

    GameObject target;

    public GameObject explosionEffect;
    public GameObject propellers;

    // speed in which the bomb goes towards the target
    public float speed = 50f;
    public float radius = 5f;
    public float force = 500f;

    float countdown;

    bool startcountdown = false;
    bool hasExploded = false;
    bool targetFound = true;
    bool searching = false;
    Vector3 pos = new Vector3(0f, 0f, 0f);

    public void Start()
    {
        //public GameObject bomb = Instantiate(Bomb, transform.position, transform.rotation);
        //public GameObject asteroid = Instantiate(Asteroid, transform.position, transform.rotation);
        // should i get this as camera position??
        pos = new Vector3(  transform.position.x, 
                            transform.position.y, 
                            transform.position.z);

        countdown = delay;

        // display on screen "searching"
        GameObject target = SeekClosest();
    }

    void Update()
    {
        if(startcountdown) {
            countdown -= Time.deltaTime;
        }

        if (searching)
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

        Vector3 apos = new Vector3( target.transform.position.x,
                                    target.transform.position.y,
                                    target.transform.position.z);
        Vector3 direction = apos - pos;
        transform.SetPositionAndRotation(pos + (speed*direction) , transform.rotation);
        // propellers
        StartPropellers();
        

        // check distance
        // if bomb is close enough, start countdown
        
    }

    void StartPropellers()
    {
        Instantiate(propellers, transform.position, transform.rotation);
    }

    GameObject SeekClosest()
    {
        Collider[] collidersSeek = Physics.OverlapSphere(transform.position, radius);
        GameObject asteroid;

        float d = 10000000;
        foreach(Collider nearbyAsteroid in collidersSeek)
        {
            Vector3 apos = new Vector3 (nearbyAsteroid.transform.position.x, 
                                        nearbyAsteroid.transform.position.y, 
                                        nearbyAsteroid.transform.position.z);
            float d1 = Vector3.Distance(pos, apos);
            if (d1 < d)
            {
                d = d1;
                // Choose what asteroid to attach to
                // maybe I have to call the parent of the collider and then get instance ID?
                asteroid = nearbyAsteroid.gameObject; //??
                searching = true;
            }
        }

        if (asteroid = null)
        {
            startcountdown = true;
            return null;
        } else
        {
            return asteroid;
        }
    }

    void Explode()
    {
        //Debug.Log("Boom");
        // show effect
        Destroy(propellers);
         
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // get nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider nearbyAsteroid in colliders)
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

