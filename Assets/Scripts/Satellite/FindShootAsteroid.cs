using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FindShootAsteroid : MonoBehaviourPun
{
    private Transform target;

    [Header("Attributes")]

    public float range = 15f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public float turnSpeed = 10f;
    public int ammo = 10;

    [Header("Unity Setup Fields")]

    public string asteroidTag = "Asteroid";
    public Transform partToRotate;

    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        SatelliteSpawner.isSatelliteActive = true;

        if (!PhotonNetwork.IsMasterClient)
        {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            transform.SetParent(ARController.Instance.earthMarker.transform);
            transform.localPosition = position;
            transform.localRotation = rotation;
        }
        Debug.Log("Satellite spawned!");

        //Ensure the method of calculating distances to targets is not continuously updated for maintainability reasons, only twice a second
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        //Create a list of all current asteroids in the game
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag(asteroidTag);

        float shortestDistance = Mathf.Infinity;
        GameObject nearestAsteroid = null;

        //Determine nearest asteroid
        foreach(GameObject asteroid in asteroids)
        {
            float distanceToAsteroid = Vector3.Distance(transform.position, asteroid.transform.position);
            if (distanceToAsteroid < shortestDistance)
            {
                shortestDistance = distanceToAsteroid;
                nearestAsteroid = asteroid;
            }
        }

        //Set nearest asteroid as target
        if (nearestAsteroid != null && shortestDistance <= range)
        {
            target = nearestAsteroid.transform;
        } else
        {
            target = null;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //If there is no target, ensure nothing is done
        if (target == null)
        {
            return;
        }

        //Coordinates that show in which direction the enemy is
        Vector3 direction = target.position - transform.position;

        //How to rotate to look in that direction
        Quaternion lookRotation = Quaternion.LookRotation(transform.InverseTransformDirection(direction), -transform.forward);

        //Convert from Quaternion to euler angle
        //Lerp for smooth transitions
        //Vector3 rotation = lookRotation.eulerAngles;
        Vector3 rotation = Quaternion.Lerp(partToRotate.localRotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;

        //Rotate around Z axis only (the gun part is rotated so Z is pointing vertically)
        partToRotate.localRotation = Quaternion.Euler(partToRotate.localRotation.eulerAngles.x, 0f, rotation.z);

        //Check if it is time to fire. If it is, shoot and reset fireCountDown
        if (fireCountdown <= 0f && ammo > 0) {
            Shoot();
            fireCountdown = 1f / fireRate;
            ammo -= 1;
        }

        //Reduce fireCountDown and satelliteTimer time
        fireCountdown -= Time.deltaTime;

    }

    void Shoot()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //When shot is fired, instantiate bullet
            GameObject LaserGO = PhotonNetwork.Instantiate("Laser", firePoint.position, firePoint.rotation);
            Laser laser = LaserGO.GetComponent<Laser>();

            //Set target of bullet in bullet script
            if (laser != null)
            {
                laser.Seek(target);
            }
        }
    }

    //Virtualization of shooting range around satellite
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
