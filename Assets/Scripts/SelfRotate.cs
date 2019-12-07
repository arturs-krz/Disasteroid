using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
    float rotSpeed = 1;
    // Start is called before the first frame update
    public GameControl GameCtrl;

	[System.Obsolete]
    private void OnMouseOver()
    {
		//Debug.Log("Here");
		//rotSpeed = 1;
		transform.Rotate(new Vector3(0, 1f, 0), rotSpeed);
        if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("onmouse");
			transform.GetComponent<Renderer>().material.color = Color.black;
		}
        if (Input.GetMouseButtonUp(0))
		{
			//rotSpeed = 0;
			transform.GetComponent<Renderer>().material.color = Color.white;
		}
    }

    private void OnMouseDown()
    {
		GameCtrl.BombAttack();
	}
}
