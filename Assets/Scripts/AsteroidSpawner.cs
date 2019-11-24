﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class AsteroidSpawner : MonoBehaviour
{
    public static int numberOfAsteroids;
    
    private bool hasEarthPosition = false;
    private System.Random rd;

    void Start()
    {
        transform.position = new Vector3(0,0,0);
        numberOfAsteroids = 0;
        rd = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (ARController.Instance.earthInstance != null) 
        {
            hasEarthPosition = true;
        }

        // Run asteroid spawn logic (and updates) only on the master client
        if (PhotonNetwork.IsMasterClient)
        {
            if (hasEarthPosition && numberOfAsteroids < 3)
            {
                float distance = 1.4f + 0.6f * (float)rd.NextDouble();

                //double theta = (Math.PI / 2) * rd.NextDouble() + Math.PI / 4;
                double phi = Math.PI * 2 * rd.NextDouble();
                //Vector3 spawnPosition = new Vector3((float)(Math.Cos(phi) * Math.Sin(theta)), (float)(Math.Cos(phi) * Math.Cos(theta)), (float)(Math.Cos(theta))) * distance;

                float xDistance = (float)Math.Cos(phi) * distance;
                float zDistance = (float)Math.Sin(phi) * distance;
                Vector3 earthPosition = ARController.Instance.earthInstance.transform.position;
                Vector3 spawnPosition = new Vector3(earthPosition.x + xDistance, earthPosition.y, earthPosition.z + zDistance);

                float x = 2 * (float)Math.PI * (float)rd.NextDouble();
                float y = 2 * (float)Math.PI * (float)rd.NextDouble();
                float z = 2 * (float)Math.PI * (float)rd.NextDouble();
                float w = 2 * (float)Math.PI * (float)rd.NextDouble();



                Vector3 angularVelocity = new Vector3((float)(Math.PI * rd.NextDouble()), (float)(Math.PI * rd.NextDouble()), (float)(Math.PI * rd.NextDouble()));

                Vector3 earthDirection = (ARController.Instance.earthInstance.transform.position - spawnPosition).normalized;
                Vector3 velocity = new Vector3(earthDirection.x + GenerateRandomOffset(0.8f), GenerateRandomOffset(0.3f), earthDirection.z + GenerateRandomOffset(0.8f)) * 0.08f;


                GameObject spawnedAsteroid = PhotonNetwork.Instantiate("Asteroid", spawnPosition, new Quaternion(x, y, z, w));
                Rigidbody rigidbody = spawnedAsteroid.GetComponent<Rigidbody>();
                rigidbody.angularVelocity = angularVelocity;
                rigidbody.velocity = velocity;

                //spawnedAsteroid.transform.localScale += spawnedAsteroid.transform.localScale * GenerateRandomOffset(0.6f);

                //float xPos = (rd.NextDouble() > 0.5 ? 1 : -1) * (0.1f + (0.2f * (float)rd.NextDouble()));
                //float zPos = (rd.NextDouble() > 0.5 ? 1 : -1) * (0.1f + (0.2f * (float)rd.NextDouble()));
                //Vector3 spawnPosition = position + new Vector3(xPos, 0, zPos);

                //GameObject spawnedAsteroid = Instantiate(asteroid, spawnPosition, Quaternion.identity);
                //Rigidbody rigidbody = spawnedAsteroid.GetComponent<Rigidbody>();
                //rigidbody.angularVelocity = angularVelocity;
                //rigidbody.velocity = velocity;

                numberOfAsteroids += 1;
            }
        }
    }

    private int RandomSign()
    {
        return rd.NextDouble() > 0.5 ? 1 : -1;
    }
    private float GenerateRandomOffset(float amplitude)
    {
        return RandomSign() * (float)rd.NextDouble() * amplitude;
    }
}
