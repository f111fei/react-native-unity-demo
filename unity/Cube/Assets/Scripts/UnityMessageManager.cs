using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MessageHandler
{
    public int id;
    public string seq;

    public String name;
    private JToken data;

    public static MessageHandler Deserialize(string message)
    {
        JObject m = JObject.Parse(message);
        MessageHandler handler = new MessageHandler(
            m.GetValue("id").Value<int>(),
            m.GetValue("seq").Value<string>(),
            m.GetValue("name").Value<string>(),
            m.GetValue("data")
        );
        return handler;
    }

    public T getData<T>()
    {
        return data.Value<T>();
    }

    public MessageHandler(int id, string seq, string name, JToken data)
    {
        this.id = id;
        this.seq = seq;
        this.name = name;
        this.data = data;
    }

    public void send(object data)
    {
        JObject o = JObject.FromObject(new
        {
            id = id,
            seq = "end",
            name = name,
            data = data
        });
        UnityMessageManager.Instance.SendMessageToRN(UnityMessageManager.MessagePrefix + o.ToString());
    }
}

public class UnityMessage
{
    public String name;
    public JObject data;
    public Action<object> callBack;
}

public class UnityMessageManager : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void onUnityMessage(string message);

    [DllImport("__Internal")]
    private static extern void logToRN(string message, int level);
#endif

    public const string MessagePrefix = "@UnityMessage@";

    private static int ID = 0;

    private static int generateId()
    {
        ID = ID + 1;
        return ID;
    }

    public static UnityMessageManager Instance { get; private set; }

    public delegate void MessageDelegate(string message);
    public event MessageDelegate OnMessage;

    public delegate void MessageHandlerDelegate(MessageHandler handler);
    public event MessageHandlerDelegate OnRNMessage;

    private Dictionary<int, UnityMessage> waitCallbackMessageMap = new Dictionary<int, UnityMessage>();

    static UnityMessageManager()
    {
        GameObject go = new GameObject("UnityMessageManager");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<UnityMessageManager>();
    }

    void Awake()
    {
    }

    public static void SetLogToRN(bool enable)
    {
        if (enable)
        {
            Application.logMessageReceived += UnityMessageManager.LogToRN;
        }
        else
        {
            Application.logMessageReceived -= UnityMessageManager.LogToRN;
        }
    }

    public static void LogToRN(string logString, string stackTrace, LogType type)
    {
        // We check for UNITY_IPHONE again so we don't try this if it isn't iOS platform.
#if UNITY_IOS && !UNITY_EDITOR
         // Now we check that it's actually an iOS device/simulator, not the Unity Player. You only get plugins on the actual device or iOS Simulator.
         if (Application.platform == RuntimePlatform.IPhonePlayer) {
             logToRN(type + ":" + logString + "\n===============\n" + stackTrace, 1);
         }
#endif
    }

    public void SendMessageToRN(string message)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass jc = new AndroidJavaClass("com.reactnative.unity.view.UnityUtils"))
            {
                jc.CallStatic("onUnityMessage", message);
            }
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
#if UNITY_IOS && !UNITY_EDITOR
            onUnityMessage(message);
#endif
        }
    }

    public void SendMessageToRN(UnityMessage message)
    {
        int id = generateId();
        if (message.callBack != null)
        {
            waitCallbackMessageMap.Add(id, message);
        }

        JObject o = JObject.FromObject(new
        {
            id = id,
            seq = message.callBack != null ? "start" : "",
            name = message.name,
            data = message.data
        });
        UnityMessageManager.Instance.SendMessageToRN(MessagePrefix + o.ToString());
    }

    void onMessage(string message)
    {
        if (OnMessage != null)
        {
            OnMessage(message);
        }
    }

    void onRNMessage(string message)
    {
        if (message.StartsWith(MessagePrefix))
        {
            message = message.Replace(MessagePrefix, "");
        }
        else
        {
            return;
        }

        MessageHandler handler = MessageHandler.Deserialize(message);
        if ("end".Equals(handler.seq))
        {
            // handle callback message
            UnityMessage m;
            if (waitCallbackMessageMap.TryGetValue(handler.id, out m))
            {
                waitCallbackMessageMap.Remove(handler.id);
                if (m.callBack != null)
                {
                    m.callBack(handler.getData<object>()); // todo
                }
            }
            return;
        }

        if (OnRNMessage != null)
        {
            OnRNMessage(handler);
        }
    }
}
