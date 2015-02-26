using UnityEngine;
using System.Collections;

public class Gyroscope : MonoBehaviour {
	
	Vector3 gSum;

	// Use this for initialization
	void Start () {
		gSum = Vector3.zero;
	}

	
	// Update is called once per frame
	void Update () {
		gSum += Arduino.gyroscope * Time.deltaTime;
		transform.rotation = Quaternion.Euler (gSum);
	}
}
