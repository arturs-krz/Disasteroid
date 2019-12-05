﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AsteroidManager : MonoBehaviour
{

    public Rigidbody rigidBody;
    public static float t = 0.02f;
    public GameObject lineRendererObject;
    private GameObject lineRendererObjInstance;
    public static float force = 0.05f;
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
        LineRenderer lr = lineRendererObjInstance.GetComponent<LineRenderer>();
        if (lr.positionCount > 0) {
            lr.positionCount -= 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);
        Vector2 coordinates = GetImpactCoordinates(other, transform.position);
        int nOfDead = GameObject.FindObjectOfType<DataManager>().Explosion(coordinates);
        Debug.Log( nOfDead + " people died");
        DataManager.totalPop -= nOfDead;
        Debug.Log("Total Population: " + DataManager.totalPop);
        Destroy(lineRendererObjInstance);
        Destroy(gameObject);
        AsteroidSpawner.numberOfAsteroids -= 1;
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        GameControl gameControl = gameController.GetComponent<GameControl>();
        gameControl.PD.value -= gameControl.PD.maxValue / 30;
        gameControl.CO2.value += gameControl.CO2.maxValue / 60;

    }

    public List<Vector3> ComputePredictedOrbit()
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 earthObjectVector = ARController.Instance.earthInstance.transform.position - transform.position;
        Vector3 acceleration = force * earthObjectVector.normalized / earthObjectVector.sqrMagnitude / rigidBody.mass;
        Vector3 velocity = rigidBody.velocity;
        Vector3 position = transform.position;
        positions.Add(position);
        float time = t;
        while (ARController.Instance.earthInstance.GetComponent<Collider>().ClosestPoint(position)!=position)
        {
            time += t;
            position += velocity * t;
            velocity += acceleration * t;
            earthObjectVector = ARController.Instance.earthInstance.transform.position - position;
            acceleration = force * earthObjectVector.normalized / earthObjectVector.sqrMagnitude / rigidBody.mass;
            positions.Add(position);

            if (positions.Count > 2000)
            {
                return positions;
            }
        }
        lineRendererObjInstance = Instantiate(lineRendererObject);
        LineRenderer lRend = lineRendererObjInstance.GetComponent<LineRenderer>();
        lRend.positionCount = positions.Count;
        Vector3[] newPos = new Vector3[positions.Count];
        for (int i = 0; i < positions.Count;i++) {
            newPos[i] = positions[positions.Count-i-1];
        }
        // Set positions of LineRenderer using linePoints array.
        lRend.SetPositions(newPos);
        return positions;
    }
    private Vector2 GetImpactCoordinates(Collider other,Vector3 position)
    {
        Quaternion rotation = other.transform.rotation;
        Vector3 relativePosition = Quaternion.Inverse(rotation) * (other.ClosestPointOnBounds(position) - other.transform.position);
        relativePosition.Normalize();
        double x = relativePosition.x;
        double y = relativePosition.y;
        double z = relativePosition.z;
        double latitude = Math.Asin(y)*180/Math.PI;
        double longitude = Math.Atan2(z, x) * 180 / Math.PI + 107f;
        if (longitude > 180f) {
            longitude -= 360f;
        }
        return new Vector2((float) latitude,(float) longitude);
    }
}



