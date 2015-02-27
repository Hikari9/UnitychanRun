using UnityEngine;
using System.Collections;

public class Gyroscope : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		transform.rotation *= Quaternion.Euler (Arduino.gyroscope * Time.deltaTime);

	}
}
