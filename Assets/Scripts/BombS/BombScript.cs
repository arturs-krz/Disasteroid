using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    GameObject asteroid;


    public GameObject explosionEffect;
    public GameObject propellers;

    // speed in which the bomb goes towards the target
    public float speed = 500f;
    public float radius = 50f;
    public float force = 500f;

    public float delay = 3f;
    float countdown;

    bool startcountdown = false;
    bool hasExploded = false;
    bool targetfound = false;

    Vector3 pos = new Vector3(0f, 0f, 0f);

    public void Start()
    {
        NetworkDebugger.Log("starting a new bomb instance_________________________________START");
        Vector3 camPos = Camera.main.gameObject.transform.position;
        //pos = Vector3.tras;

        countdown = delay;
        // display on screen "searching"
        asteroid = SeekClosest();
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
            Intercept(asteroid);
        }
    }

    void Intercept(GameObject target)
    {
        Vector3 apos = target.transform.position;

        //Vector3 direction = apos - pos;
        //transform.SetPositionAndRotation(pos - (speed*direction) , transform.rotation);

        // check if this works
        float delta = 1f;
        Vector3.MoveTowards(pos, apos, delta); 



        //StartPropellers();
        if (Vector3.Distance(apos, pos) < 1f) {
            if (startcountdown == false)
            {
                NetworkDebugger.Log("target, locked, starting countodwn");
                startcountdown = true;
            }
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
        //foreach(GameObject nearbyAsteroid in AsteroidSpawner.Instance.asteroids)
        //{
        //    NetworkDebugger.Log("found another target");
        //    Vector3 apos = nearbyAsteroid.transform.position;
        //    float d1 = Vector3.Distance(pos, apos);
        //    if (d1 < d)
        //    {
        //        d = d1;
        //        asteroid = nearbyAsteroid; 
        //    }
        //}
        targetfound = true;

        NetworkDebugger.Log("decided on a target");
        return asteroid;
    }

    void Explode()
    {
        //Destroy(propellers);
        NetworkDebugger.Log("BOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOM");
        
        Instantiate(explosionEffect, transform.position, transform.rotation);

        //foreach(GameObject nearbyAsteroid in AsteroidSpawner.Instance.asteroids)
        //{
        //    // add force -> move
        //    Rigidbody rb = nearbyAsteroid.GetComponent<Rigidbody>();
        //    if(rb != null)
        //    {
        //        rb.AddExplosionForce(force, transform.position, radius);
        //    }
        //    // damage
        //}
        // remove bomb
        Destroy(gameObject);
    }
}

