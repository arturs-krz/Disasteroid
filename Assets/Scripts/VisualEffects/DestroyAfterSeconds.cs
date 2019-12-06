using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyAfterSeconds : MonoBehaviour
{
    private float timer;
    public float decayTime;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > decayTime)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
