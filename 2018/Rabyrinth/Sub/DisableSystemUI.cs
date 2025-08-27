using UnityEngine;

public class DisableSystemUI
{
#if UNITY_ANDROID
    public AndroidJavaObject activityInstance;
    public AndroidJavaObject windowInstance;
    public AndroidJavaObject viewInstance;

    public const int SYSTEM_UI_FLAG_HIDE_NAVIGATION = 2;
    public const int SYSTEM_UI_FLAG_LAYOUT_STABLE = 256;
    public const int SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION = 512;
    public const int SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN = 1024;
    public const int SYSTEM_UI_FLAG_IMMERSIVE = 2048;
    public const int SYSTEM_UI_FLAG_IMMERSIVE_STICKY = 4096;
    public const int SYSTEM_UI_FLAG_FULLSCREEN = 4;

    public void Run()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        activityInstance.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            if (viewInstance != null)
            {
                viewInstance.Call("setSystemUiVisibility",
                                  SYSTEM_UI_FLAG_LAYOUT_STABLE
                                  | SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION
                                  | SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN
                                  | SYSTEM_UI_FLAG_HIDE_NAVIGATION
                                  | SYSTEM_UI_FLAG_FULLSCREEN
                                  | SYSTEM_UI_FLAG_IMMERSIVE_STICKY
                                  );
            }
        }));
#endif
    }

#endif

    public void DisableNavUI()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

#if UNITY_ANDROID
        using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
            windowInstance = activityInstance.Call<AndroidJavaObject>("getWindow");
            viewInstance = windowInstance.Call<AndroidJavaObject>("getDecorView");
        }
#endif
    }
}