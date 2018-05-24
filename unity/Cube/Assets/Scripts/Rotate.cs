﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    private bool canRotate = true;

    // Use this for initialization
    void Awake()
    {
        UnityMessageManager.Instance.OnUnityMessage += onMessage;
    }

    void onDestroy()
    {
        UnityMessageManager.Instance.OnUnityMessage -= onMessage;
    }

    void onMessage(MessageHandler message)
    {
        var data = message.getData<string>();
        Debug.Log("onMessage:" + data);
        canRotate = !canRotate;
        message.send(new { CallbackTest = "I am Unity callback" });
    }

    void OnMouseDown()
    {
        Debug.Log("click");
        UnityMessageManager.Instance.SendMessageToRN(new UnityMessage()
        {
            name = "click",
            callBack = (data) =>
            {
                Debug.Log("onClickCallBack:" + data);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!canRotate)
        {
            return;
        }
        var delta = 30 * Time.deltaTime;
        transform.localRotation *= Quaternion.Euler(delta, delta, delta);
    }
}
