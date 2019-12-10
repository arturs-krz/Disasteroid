using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombThrower : MonoBehaviour
{
    public GameObject Bomb;

    public void BombThrowing()
    {
        NetworkDebugger.Log("Clikcing on the UI");
        // not sure if I should get position and rotation of the camera instead of the button ??
        GameObject bomb = Instantiate(Bomb, Camera.main.transform.position, Quaternion.identity);
    }
}
