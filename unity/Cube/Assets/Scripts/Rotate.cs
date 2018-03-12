using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    private bool canRotate = true;

    // Use this for initialization
    void Awake()
    {
        UnityMessageManager.Instance.OnMessage += toggleRotate;
    }

    void onDestroy()
    {
		UnityMessageManager.Instance.OnMessage -= toggleRotate;
    }

    void toggleRotate(string message)
    {
        Debug.Log("onMessage:" + message);
        canRotate = !canRotate;
    }

    void OnMouseDown()
    {
        Debug.Log("click");
        UnityMessageManager.Instance.SendMessageToRN("click");
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
