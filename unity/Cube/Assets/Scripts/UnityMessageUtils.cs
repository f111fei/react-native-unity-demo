using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMessageUtils
{

    public static void SendMessageToRN(string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.reactnative.unity.view.UnityUtils"))
            {
                jc.CallStatic("onUnityMessage", message);
            }
        }
    }
}
