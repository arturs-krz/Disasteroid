using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AsteroidManager : MonoBehaviour
{

    public Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position + AsteroidSpawner.position;
        this.rigidBody.AddForce(-10000 * pos.normalized / pos.sqrMagnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);    
        Destroy(gameObject);
        AsteroidSpawner.numberOfAsteroids -= 1;
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        GameControl gameControl = gameController.GetComponent<GameControl>();
        gameControl.PD.value += gameControl.PD.maxValue / 30;
        gameControl.CO2.value += gameControl.CO2.maxValue / 60;

    }
}
