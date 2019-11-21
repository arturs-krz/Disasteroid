using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AsteroidSpawner : MonoBehaviour
{
    public static int numberOfAsteroids;
    public GameObject asteroid;
    public static Vector3 position = new Vector3(0,0,0);
    
    private bool hasEarthPosition = false;

    void Start()
    {
        transform.position = position;
        numberOfAsteroids = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (ARController.Instance.earthInstance != null) 
        {
            position = ARController.Instance.earthInstance.transform.position;
            hasEarthPosition = true;
        }

        if (hasEarthPosition && numberOfAsteroids <= 10)
        {
            System.Random rd = new System.Random();
            // float distance = 50 + 50 * (float) rd.NextDouble();
            
            // double theta = (Math.PI/2) * rd.NextDouble() + Math.PI/4;
            // double phi = Math.PI * 2 * rd.NextDouble();
            // Vector3 spawnPosition = new Vector3((float) (Math.Cos(phi) * Math.Sin(theta)), (float) (Math.Cos(phi)*Math.Cos(theta)) ,(float) (Math.Cos(theta))) * distance;
            // float x = 2* (float) Math.PI * (float)rd.NextDouble();
            // float y = 2 * (float)Math.PI * (float)rd.NextDouble();
            // float z = 2 * (float)Math.PI * (float)rd.NextDouble();
            // float w = 2 * (float)Math.PI * (float)rd.NextDouble();



            Vector3 angularVelocity = new Vector3((float) (Math.PI * rd.NextDouble()), (float)(Math.PI * rd.NextDouble()), (float)(Math.PI * rd.NextDouble()));
            // Vector3 velocity = new Vector3((float)(rd.NextDouble()), (float)(rd.NextDouble()), (float)(rd.NextDouble())) * 0.2f;


            // GameObject spawnedAsteroid = Instantiate(asteroid, spawnPosition, new Quaternion(x, y, z, w));
            // Rigidbody rigidbody = spawnedAsteroid.GetComponent<Rigidbody>();
            // rigidbody.angularVelocity = angularVelocity;
            // rigidbody.velocity = velocity;


            float xPos = (rd.NextDouble() > 0.5 ? 1 : -1) * (0.1f + (0.2f * (float)rd.NextDouble()));
            float zPos = (rd.NextDouble() > 0.5 ? 1 : -1) * (0.1f + (0.2f * (float)rd.NextDouble()));
            Vector3 spawnPosition = position + new Vector3(xPos, 0, zPos);

            GameObject spawnedAsteroid = Instantiate(asteroid, spawnPosition, Quaternion.identity);
            Rigidbody rigidbody = spawnedAsteroid.GetComponent<Rigidbody>();
            rigidbody.angularVelocity = angularVelocity;
            //rigidbody.velocity = velocity;

            numberOfAsteroids += 1;
        }
    }
}
