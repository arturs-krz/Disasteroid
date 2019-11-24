using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopClient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.Log("We're not on Android! Could it be the PC? :o");

            // Disable the AR specific GameObjects
            GameObject ARDevice = GameObject.Find("ARCore Device");
            ARDevice.SetActive(false);

            //GameObject ARController = GameObject.Find("Diasteroid AR Controller");
            //ARController.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
