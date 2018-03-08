using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	private bool canRotate = true;

	// Use this for initialization
	void Start () {
		
	}

	void toggleRotate(string message) {
		Debug.Log("onMessage:" + message);
		canRotate = !canRotate;
	}
	
	// Update is called once per frame
	void Update () {
		if (!canRotate) {
			return;
		}
		var delta = 30 * Time.deltaTime;
		transform.localRotation *= Quaternion.Euler(delta, delta, delta);
	}
}
