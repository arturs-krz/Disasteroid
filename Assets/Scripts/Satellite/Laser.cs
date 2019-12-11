using UnityEngine;
using Photon.Pun;

public class Laser : MonoBehaviour
{

    private Transform target;

    //Set bullet speed
    public float speed = 35f;

    private Vector3 dir;

    //Set target
    public void Seek(Transform _target)
    {
        target = _target;
    }

    // Update is called once per frame
    void Update()
    {
        //Ensure that if target disappears, so does the bullet after some time
        if (target == null)
        {
            PhotonNetwork.Destroy(gameObject);
            return;
        }

        //Set direction of bullet, if target has disappeared, remain going in same direction for some time
        Vector3 dir = target.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        //Determine if bullet has reached target
        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        //Have bullet move forward
        //Normalized to ensure that movement speed is constant, regardless of distance
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    void HitTarget()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Upon target hit, initiate special effect
            GameObject visualEffect = PhotonNetwork.Instantiate("DustExplosion", transform.position, transform.rotation);
            
            //Destroy special effect, bullet and asteroid
            PhotonNetwork.Destroy(target.gameObject);
            AsteroidSpawner.numberOfAsteroids -= 1;

            PhotonNetwork.Destroy(gameObject);
        }
    }
}
