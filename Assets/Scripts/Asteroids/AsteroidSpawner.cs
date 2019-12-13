using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class AsteroidSpawner : MonoBehaviour
{
    private static AsteroidSpawner _instance;
    public static AsteroidSpawner Instance { get { return _instance; } }

    public static int numberOfAsteroids;

    public static List<GameObject> asteroidInstances = new List<GameObject>();

    public List<GameObject> asteroidPrefabs;
    
    private bool hasEarthPosition = false;
    private System.Random rd;

    private int MAX_ASTEROIDS = 3;
    private float spawnTimer = 3f;
    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

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
            if (PhotonGameLobby.Instance.numPlayers > 0 && numberOfAsteroids < MAX_ASTEROIDS)
            {
                spawnTimer -= Time.deltaTime;
            }

            if (hasEarthPosition
                && numberOfAsteroids < MAX_ASTEROIDS
                && PhotonGameLobby.Instance.numPlayers > 0
                && spawnTimer < 0f
                && !PopVegManager.gameOver
            )
            {
                float distance = 1.8f + 0.6f * (float)rd.NextDouble();

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
                
                string randomAsteroidPrefabName = asteroidPrefabs[rd.Next(asteroidPrefabs.Count)].name;
                GameObject spawnedAsteroid = PhotonNetwork.Instantiate(randomAsteroidPrefabName, spawnPosition, new Quaternion(x, y, z, w));
                
                Rigidbody rigidbody = spawnedAsteroid.GetComponent<Rigidbody>();
                rigidbody.angularVelocity = angularVelocity;
                rigidbody.velocity = velocity;


                Vector3 asteroidScale = spawnedAsteroid.transform.localScale + (spawnedAsteroid.transform.localScale * ((float)rd.NextDouble() * 0.5f));
                spawnedAsteroid.GetComponent<Asteroid>().SetInitialScale(asteroidScale);

                numberOfAsteroids += 1;
                if (numberOfAsteroids < MAX_ASTEROIDS)
                {
                    spawnTimer = 2f + GenerateRandomOffset(2f);
                }
                else
                {
                    spawnTimer = 1f + GenerateRandomOffset(0.5f);
                }

                //float xPos = (rd.NextDouble() > 0.5 ? 1 : -1) * (0.1f + (0.2f * (float)rd.NextDouble()));
                //float zPos = (rd.NextDouble() > 0.5 ? 1 : -1) * (0.1f + (0.2f * (float)rd.NextDouble()));
                //Vector3 spawnPosition = position + new Vector3(xPos, 0, zPos);

                //GameObject spawnedAsteroid = Instantiate(asteroid, spawnPosition, Quaternion.identity);
                //Rigidbody rigidbody = spawnedAsteroid.GetComponent<Rigidbody>();
                //rigidbody.angularVelocity = angularVelocity;
                //rigidbody.velocity = velocity;              
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