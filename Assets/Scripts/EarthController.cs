using UnityEngine;
using System.Collections;

/// <summary>
/// EarthController
/// We use this as a regular object (NOT networked) as it is
/// instantiated seperately for each client.
/// </summary>
public class EarthController : MonoBehaviour {
    private float speed = 0.15f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Set angular velocity to the rigidbody so we don't have to
        // update all the time and can only
        // occasionally check whether the current rotation is the same
        // between clients.
        Vector3 angularVel = rb.transform.up.normalized * speed;
        rb.angularVelocity = angularVel;


    }

    void Update()
    {
        //transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
        //Debug.Log(rb.angularVelocity);
    }

}