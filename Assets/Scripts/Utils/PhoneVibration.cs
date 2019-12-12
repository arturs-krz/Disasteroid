using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Source: https://www.reddit.com/r/Unity3D/comments/4j5js7/unity_vibrate_android_device_for_custom_duration/
public class PhoneVibration : MonoBehaviour
{
    private static AndroidJavaObject Vibrator = null;

    static PhoneVibration()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Vibrator = new AndroidJavaClass("com.unity3d.player.UnityPlayer")// Get the Unity Player.
                .GetStatic<AndroidJavaObject>("currentActivity")// Get the Current Activity from the Unity Player.
                .Call<AndroidJavaObject>("getSystemService", "vibrator");// Then get the Vibration Service from the Current Activity.
        }

        // Trick Unity into giving the App vibration permission when it builds.
        // This check will always be false, but the compiler doesn't know that.
        if (Application.isEditor) Handheld.Vibrate();
    }

    public static void Vibrate(long milliseconds)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Vibrator.Call("vibrate", milliseconds);
        }
    }

    public static void Vibrate(long[] pattern, int repeat)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Vibrator.Call("vibrate", pattern, repeat);
        }
    }
}
