using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public static int numberOfAsteroids = 0;
    public GameObject asteroid;
    public static Vector3 position = new Vector3(20,0,0);
    // Start is called before the first frame update
    void Start()
    {
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfAsteroids == 0)
        {
            Instantiate(asteroid, transform);
            numberOfAsteroids += 1;
        }
    }
}
