using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("fuck fuck fuck");
		var delta = 30 * Time.deltaTime;
		transform.localRotation *= Quaternion.Euler(delta, delta, delta);
	}
}
