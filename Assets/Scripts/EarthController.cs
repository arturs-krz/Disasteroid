using UnityEngine;
using System.Collections;

public class EarthController : MonoBehaviour {
    public float speed = 0.00001f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 angularVel = rb.transform.up.normalized * 0.25f;
        Debug.Log(angularVel);

        rb.angularVelocity = angularVel;
    }

    void Update()
    {
        //transform.Rotate(Vector3.up, speed * Time.deltaTime, Space.World);
        //Debug.Log(rb.angularVelocity);
    }

}