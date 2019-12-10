using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = GoogleARCore.InstantPreviewInput;
#endif


public class ARController : MonoBehaviour
{
    private static ARController _instance;
    public static ARController Instance { get { return _instance; } }

    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR
    /// background).
    /// </summary>
    public Camera FirstPersonCamera;

    public GameObject earthPrefab;
    public GameObject earthInstance { get; private set; }

    public Transform anchorTransform { get; private set; }

    public GameObject earthMarker { get; private set; }

    private GameObject earthSync;
    private bool fedLatestInstance = false; // whether latest earth instance is fed to the sync object

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error,
    /// otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    private bool isAndroid = true;

    private float asteroidTriggerTimer = 0f;

    private List<AugmentedImage> trackedImages = new List<AugmentedImage>();

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        isAndroid = Application.platform == RuntimePlatform.Android;
        if (!isAndroid)
        {
            // If we're not on Android, insantiate earth at origin.
            earthInstance = Instantiate(earthPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        if (!isAndroid) {
            if (PhotonGameLobby.Instance.connected && earthSync == null)
            {
                earthSync = PhotonNetwork.Instantiate("EarthSyncDummy", new Vector3(0, 0, 0), Quaternion.identity);
                earthSync.GetComponent<EarthRotationSync>().FeedEarthInstance(earthInstance);

                // Once we have instantiated everything on the desktop master client,
                // we can disable the component.
                gameObject.SetActive(false);
            }
            return;
        }

        UpdateApplicationLifecycle();

        Session.GetTrackables<AugmentedImage>(trackedImages, TrackableQueryFilter.Updated);

        foreach (AugmentedImage image in trackedImages)
        {    
            switch (image.Name)
            {
                case "Earth":
                    if (image.TrackingState == TrackingState.Tracking && earthInstance == null)
                    {
                        Anchor imageAnchor = image.CreateAnchor(image.CenterPose);
                        //imageAnchors.Add(image.DatabaseIndex, imageAnchor);

                        earthInstance = Instantiate(earthPrefab, imageAnchor.transform);
                        earthInstance.transform.Translate(new Vector3(0, 0.3f, 0), Space.World);
                        anchorTransform = imageAnchor.transform;

                        earthMarker = Instantiate(new GameObject("EarthMarker"), imageAnchor.transform);
                        earthMarker.transform.localScale = new Vector3(1f,1f,1f);
                        earthMarker.transform.position = earthInstance.transform.position;

                        GameObject environmentalLight = GameObject.FindWithTag("EnvironmentalLight");
                        environmentalLight.transform.SetParent(earthMarker.transform);
                        environmentalLight.transform.localPosition = new Vector3(0,0,-1);

                        fedLatestInstance = false;

                        // When on mobile, join the game only after we've insantiated the earth
                        // and know the world origin.
                        PhotonGameLobby.Instance.JoinGame();

                    }
                    else if (image.TrackingState == TrackingState.Stopped)
                    {
                        Destroy(earthInstance);
                        Destroy(earthMarker);
                    }
                    break;
                case "Satellite":
                    if (image.TrackingState == TrackingState.Paused)
                    {
                        if (asteroidTriggerTimer == 0f)
                        {
                            asteroidTriggerTimer = Time.time;
                        }
                        else if (Time.time - asteroidTriggerTimer > 2f)
                        {
                            NetworkDebugger.Log("LOOKING FOR 2 SECONDS");
                            asteroidTriggerTimer = Time.time;
                            SatelliteSpawner.SpawnSatellite(image.CenterPose);
                        }
                    }

                    // if (image.TrackingState == TrackingState.Tracking 
                    //     && image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking
                    //     // && (image.CenterPose.position - FirstPersonCamera.transform.position).magnitude < 2f
                    // )
                    // {
                    //     NetworkDebugger.Log("TRACKING SATELLITE");
                    //     //NetworkDebugger.Log(Vector3.Dot(FirstPersonCamera.transform.forward, image.CenterPose.up));
                    //     // if (asteroidTriggerTimer == 0f)
                    //     // {
                    //     //     asteroidTriggerTimer = Time.time;
                    //     // }
                    //     // else if (Time.time - asteroidTriggerTimer > 0.5f)
                    //     // {
                    //     //     NetworkDebugger.Log("LOOKING FOR 3 SECONDS");
                    //     //     asteroidTriggerTimer = Time.time;
                    //     //     PhotonNetwork.Instantiate("AR-Satellite", FirstPersonCamera.transform.position, Quaternion.identity);
                    //     // }
                    //     SatelliteSpawner.SpawnSatellite(image.CenterPose);
                    // }
                    // else
                    // {
                    //     if (image.TrackingState == TrackingState.Paused)
                    //     {
                    //         if (asteroidTriggerTimer == 0f)
                    //         {
                    //             asteroidTriggerTimer = Time.time;
                    //         }
                    //         else if (Time.time - asteroidTriggerTimer > 2f)
                    //         {
                    //             NetworkDebugger.Log("LOOKING FOR 3 SECONDS");
                    //             asteroidTriggerTimer = Time.time;
                    //             PhotonNetwork.Instantiate("AR-Satellite", FirstPersonCamera.transform.position, Quaternion.identity);
                    //         }
                    //     }
                    //     //NetworkDebugger.Log("LOOKED AWAY");
                    //     asteroidTriggerTimer = 0f;
                    // }
                    break;
            }
            
        }

        if (earthSync == null)
        {
            earthSync = GameObject.FindWithTag("EarthSync");
        }
        else if (earthSync != null && earthInstance != null && !fedLatestInstance)
        {
            earthSync.GetComponent<EarthRotationSync>().FeedEarthInstance(earthInstance);
            fedLatestInstance = true;
        }
    }

    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to
        // appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject =
                    toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
