using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AsteroidManager : MonoBehaviour
{

    public Rigidbody rigidBody;
    public static float t = 0.02f;

    public float force = 0.95f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = ARController.Instance.earthInstance.transform.position - transform.position;
        this.rigidBody.AddForce(force * pos.normalized / pos.sqrMagnitude);
        //this.rigidBody.AddForce(-1 * pos.normalized / pos.sqrMagnitude);

        // Destroy if it gets away
        if (pos.magnitude > 2.2f)
        {
            Destroy(gameObject);
            AsteroidSpawner.numberOfAsteroids -= 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);    
        Destroy(gameObject);
        Vector2 coordinates = GetImpactCoordinates(other);
        AsteroidSpawner.numberOfAsteroids -= 1;
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        GameControl gameControl = gameController.GetComponent<GameControl>();
        gameControl.PD.value -= gameControl.PD.maxValue / 30;
        gameControl.CO2.value += gameControl.CO2.maxValue / 60;

    }

    List<Vector3> ComputePredictedOrbit(out float time)
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 earthObjectVector = ARController.Instance.earthInstance.transform.position - transform.position;
        Vector3 acceleration = force * earthObjectVector.normalized / earthObjectVector.sqrMagnitude / rigidBody.mass;
        Vector3 velocity = rigidBody.velocity;
        Vector3 position = transform.position;
        positions.Add(position);
        time = t;
        while (true)
        {
            time += t;
            position += velocity * t;
            velocity += acceleration * t;
            earthObjectVector = ARController.Instance.earthInstance.transform.position - position;
            acceleration = force * earthObjectVector.normalized / earthObjectVector.sqrMagnitude / rigidBody.mass;
            positions.Add(position);
            if(earthObjectVector.sqrMagnitude< 1)
            {
                break;
            }
            if (positions.Count > 2000)
            {
                break;
            }
        }
        return positions;
    }
    private Vector2 GetImpactCoordinates(Collider other)
    {
        Vector3 relativePosition = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position) - other.gameObject.GetComponent<Collider>().transform.position;
        return new Vector2();
    }
}



