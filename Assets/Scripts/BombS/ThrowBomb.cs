using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBomb : MonoBehaviour
{
    readonly GameObject Bomb;
    
    // Start is called before the first frame update
    public void BombThrowing()
    {
        // not sure if I should get position and rotation of the camera instead of the button ??
        GameObject bomb = Instantiate(Bomb, transform.position, transform.rotation);


        // i don't know if this works like this or only instantiating it's enough
        BombScript bs = bomb.GetComponent<BombScript>();
        bs.Start();
    }

}
