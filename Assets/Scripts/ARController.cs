using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
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

    private GameObject earthSync;

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error,
    /// otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    private bool isAndroid = true;

    private List<AugmentedImage> trackedImages = new List<AugmentedImage>();
    //public Dictionary<int, Anchor> imageAnchors = new Dictionary<int, Anchor>();

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
            // If we're not on Android, insantiate earth at origin and disable GameObject
            earthInstance = Instantiate(earthPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            earthSync = PhotonNetwork.Instantiate("EarthSyncDummy", new Vector3(0, 0, 0), Quaternion.identity);

            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        UpdateApplicationLifecycle();

        Session.GetTrackables<AugmentedImage>(trackedImages, TrackableQueryFilter.Updated);

        foreach (AugmentedImage image in trackedImages)
        {
            //Debug.Log(image.DatabaseIndex);

            //Anchor imageAnchor;
            //imageAnchors.TryGetValue(image.DatabaseIndex, out imageAnchor);
            
            if (image.TrackingState == TrackingState.Tracking && earthInstance == null)
            {
                Anchor imageAnchor = image.CreateAnchor(image.CenterPose);
                //imageAnchors.Add(image.DatabaseIndex, imageAnchor);

                earthInstance = Instantiate(earthPrefab, imageAnchor.transform);
                earthInstance.transform.Translate(new Vector3(0, 0.3f, 0), Space.World);

                // When on mobile, join the game only after we've insantiated the earth
                // and know the world origin.
                PhotonGameLobby.Instance.JoinGame();
                //PhotonNetwork.JoinRoom("DiasteroidMain");
            }
            else if (image.TrackingState == TrackingState.Stopped)
            {
                Destroy(earthInstance);
            }
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
