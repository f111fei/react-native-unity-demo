using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UnityMessageUtils
{
    #if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void onUnityMessage(string message);
    #endif

    public static void SendMessageToRN(string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.reactnative.unity.view.UnityUtils"))
            {
                jc.CallStatic("onUnityMessage", message);
            }
        } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
            #if UNITY_IPHONE && !UNITY_EDITOR
                onUnityMessage(message);
            #endif
        }
    }
}
