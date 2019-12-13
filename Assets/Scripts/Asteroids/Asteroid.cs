using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Asteroid : MonoBehaviourPun, IPunObservable
{
    // Set the visual impact effects
    private GameObject visualEffect;

    // Variables for resources
    private GameObject ResourceManager;
    private CO2Manager CO2Manage;

    private Rigidbody rb;

    /// <summary>
    /// The initial scaling of the asteroid when it's spawned.
    /// Used to calculate the forced perspective afterwards
    /// </summary>
    private Vector3 baseScale;

    /// <summary>
    /// Relative Earth position. Vector from the asteroid to Earth.
    /// </summary>
    private Vector3 earthPos;

    /// <summary>
    /// Initial relative Earth position.
    /// Used to calculate the forced perspective.
    /// </summary>
    private Vector3 initialEarthPos;

    public static float t = 0.02f;

    // Line renderer prefab for rendering the asteroid's path
    public GameObject lineRendererPrefab;
    private GameObject lineRendererObjInstance = null;
    private LineRenderer pathLineRenderer;
    private bool needsOrbitRecalculation = false;

    private PopVegManager popVegManager;

    public static float force = 0.05f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();     
    }

    void Start()
    {
        // Set CO2 variables
        ResourceManager = GameObject.FindGameObjectWithTag("ResourceManager");
        CO2Manage = ResourceManager.GetComponent<CO2Manager>();
        popVegManager = GameObject.FindObjectOfType<PopVegManager>();

        if (photonView.IsMine)
        {
            // Get the position of the Earth instance and calculate the position vector against it.
            earthPos = ARController.Instance.earthInstance.transform.position - transform.position;
            initialEarthPos = earthPos;

            baseScale = transform.localScale;

            ComputePredictedOrbit();

            //List<Vector3> pathPositions = ComputePredictedOrbit();
            //if (pathPositions.Count < 2000)
            //{
            //    AsteroidSpawner.numberOfAsteroids += 1;
            //}
            //else {
            //    PhotonNetwork.Destroy(gameObject);
            //}
        }
        else
        {
            transform.SetParent(ARController.Instance.earthMarker.transform);

            // Remove collider to save cpu as it's not needed for phone clients 
            Destroy(GetComponent<Collider>());         
        }
        AsteroidSpawner.asteroidInstances.Add(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        PhoneVibration.Vibrate(200);
        Destroy(pathLineRenderer);
        Destroy(lineRendererObjInstance);
        AsteroidSpawner.asteroidInstances.Remove(gameObject);
    }

    /// <summary>
    /// Manually specify the initial scale of the asteroid.
    /// </summary>
    /// <param name="scale">transform.localScale</param>
    public void SetInitialScale(Vector3 scale)
    {
        baseScale = scale;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // This will be true for the master client who is spawning the asteroids
        if (stream.IsWriting)
        {
            // Send all the physics properties from the master simulation.
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
            stream.SendNext(transform.localScale);

            stream.SendNext(transform.position);
            stream.SendNext(rb.rotation);
        }
        else
        {
            // Sync on the other clients
            
            rb.velocity = ARController.Instance.earthMarker.transform.TransformDirection((Vector3)stream.ReceiveNext());
            // rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();

            transform.localScale = (Vector3)stream.ReceiveNext();

            // lastPosition = (Vector3)stream.ReceiveNext();
            // lastRotation = (Quaternion)stream.ReceiveNext();

            // lastPosition = lastPosition + ARController.Instance.earthInstance.transform.position;

            transform.localPosition = (Vector3)stream.ReceiveNext();
            rb.rotation = (Quaternion)stream.ReceiveNext();

            // Calculate the time delta (lag) between current server time and when the latest update was sent
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            // Compensate position
            transform.localPosition = transform.localPosition + ARController.Instance.earthMarker.transform.InverseTransformDirection(rb.velocity * lag);

            if (lineRendererObjInstance == null || needsOrbitRecalculation)
            {
                // Compute orbit locally on the first network update
                ComputePredictedOrbitLocal();
                pathLineRenderer.useWorldSpace = false;
                lineRendererObjInstance.transform.SetParent(ARController.Instance.earthMarker.transform);
                lineRendererObjInstance.transform.localPosition = new Vector3(0,0,0);
                lineRendererObjInstance.transform.localRotation = Quaternion.identity;
                needsOrbitRecalculation = false;
            }
        }
    }

    void FixedUpdate()
    {
        // On the master client (IsMine true for the owner of object)
        if (photonView.IsMine)
        {
            earthPos = ARController.Instance.earthInstance.transform.position - transform.position;
            rb.AddForce(force * earthPos.normalized / earthPos.sqrMagnitude);

            // Destroy if it gets away
            if (earthPos.magnitude > 2.2f)
            {
                PhotonNetwork.Destroy(gameObject);
                AsteroidSpawner.numberOfAsteroids -= 1;
            }

            // Forced perspective. Make it appear slightly smaller if it's close to earth.
            transform.localScale = baseScale * Mathf.Clamp((earthPos.magnitude / initialEarthPos.magnitude) + 0.5f, 0.5f, 1.0f);

            if (needsOrbitRecalculation)
            {
                needsOrbitRecalculation = false;
                ComputePredictedOrbit();
            }
        }

        if (pathLineRenderer != null && pathLineRenderer.positionCount > 0) {
            pathLineRenderer.positionCount -= 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            // If asteroid hits water, play waterEffect, else play earthEffect
            if (other.tag == "Water")
            {
                Vector3 collisionNormal = (transform.position - ARController.Instance.earthInstance.transform.position).normalized;
                Quaternion normalRotation = Quaternion.LookRotation(collisionNormal, Vector3.up) * Quaternion.Euler(90f, 0, 0);

                // SmallSplash was used for direction testing
                //visualEffect = PhotonNetwork.Instantiate("SmallSplash", transform.position, normalRotation);
                visualEffect = PhotonNetwork.Instantiate("BigSplash", transform.position, normalRotation);
            }
            else
            {
                visualEffect = PhotonNetwork.Instantiate("DustExplosion", transform.position, transform.rotation);
            }

            // Kill people upon impact
            Vector2 coordinates = GetImpactCoordinates(other, transform.position);
            long nOfDead;
            float dead_veg;
            popVegManager.Explosion(coordinates, out nOfDead, out dead_veg);
            //Debug.Log( nOfDead + " people died, " + "Total Population: " + PopVegManager.totalPop +"; " + dead_veg + " vegetation index burned, Remaining vegetation index: " + PopVegManager.totalVeg);

            // Increase CO2 value upon impact
            if (CO2Manage.currentCO2 < (CO2Manage.maxCO2-CO2Manage.impactCO2))
            {
                CO2Manage.currentCO2 += CO2Manage.impactCO2;
            }

            // Destroy asteroid after impact
            PhotonNetwork.Destroy(gameObject);
            AsteroidSpawner.numberOfAsteroids -= 1;
        }
    }

    public void AffectByExplosion(float force, Vector3 position, float radius)
    {
        if (photonView.IsMine)
        {
            rb.AddExplosionForce(force, position, radius, 0f, ForceMode.Impulse);
            photonView.RPC("RecalculateOrbitPath", RpcTarget.Others);

            needsOrbitRecalculation = true;
        }
    }

    [PunRPC]
    void RecalculateOrbitPath()
    {
        needsOrbitRecalculation = true;
    }

    public List<Vector3> ComputePredictedOrbit()
    {
        List<Vector3> positions = new List<Vector3>();
        
        Vector3 earthObjectVector = ARController.Instance.earthInstance.transform.position - transform.position;
        Vector3 acceleration = force * earthObjectVector.normalized / earthObjectVector.sqrMagnitude / rb.mass;
        Vector3 velocity = rb.velocity;
        Vector3 position = transform.position;

        positions.Add(position);
        float time = t;

        Collider earthCollider = ARController.Instance.earthInstance.transform.GetComponentInChildren<SphereCollider>();
        while (earthCollider.ClosestPoint(position) != position)
        {
            time += t;
            position += velocity * t;
            velocity += acceleration * t;
            earthObjectVector = ARController.Instance.earthInstance.transform.position - position;
            acceleration = force * earthObjectVector.normalized / earthObjectVector.sqrMagnitude / rb.mass;
            positions.Add(position);

            if (positions.Count > 2000)
            {
                return positions;
            }
        }
        if (lineRendererObjInstance != null) {
            Destroy(lineRendererObjInstance);
        }
        lineRendererObjInstance = Instantiate(lineRendererPrefab);
        
        pathLineRenderer = lineRendererObjInstance.GetComponent<LineRenderer>();
        pathLineRenderer.positionCount = positions.Count;

        Vector3[] newPos = new Vector3[positions.Count];
        for (int i = 0; i < positions.Count;i++) {
            newPos[i] = positions[positions.Count-i-1];
        }
        // Set positions of LineRenderer using linePoints array.
        pathLineRenderer.SetPositions(newPos);
        return positions;
    }

    public List<Vector3> ComputePredictedOrbitLocal()
    {
        List<Vector3> positions = new List<Vector3>();

        Vector3 earthObjectVector = ARController.Instance.earthInstance.transform.position - transform.position;
        Vector3 acceleration = force * earthObjectVector.normalized / earthObjectVector.sqrMagnitude / rb.mass;
        Vector3 velocity = rb.velocity;

        Vector3 position = transform.position;
        Transform parentTransform = transform.parent;

        positions.Add(transform.localPosition);
        float time = t;

        Collider earthCollider = ARController.Instance.earthInstance.transform.GetComponentInChildren<SphereCollider>();
        while (earthCollider.ClosestPoint(position) != position)
        {
            time += t;
            position += velocity * t;
            velocity += acceleration * t;
            earthObjectVector = ARController.Instance.earthInstance.transform.position - position;
            acceleration = force * earthObjectVector.normalized / earthObjectVector.sqrMagnitude / rb.mass;
            positions.Add(parentTransform.InverseTransformPoint(position));

            if (positions.Count > 2000)
            {
                return positions;
            }
        }
        if (lineRendererObjInstance != null)
        {
            Destroy(lineRendererObjInstance);
        }
        lineRendererObjInstance = Instantiate(lineRendererPrefab);

        pathLineRenderer = lineRendererObjInstance.GetComponent<LineRenderer>();
        pathLineRenderer.positionCount = positions.Count;

        // Set positions of LineRenderer using linePoints array.
        positions.Reverse();
        pathLineRenderer.SetPositions(positions.ToArray());
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
