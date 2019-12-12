using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyAfterSeconds : MonoBehaviourPun
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

        if (photonView.IsMine && timer > decayTime)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
