using UnityEngine;
using System.Collections;

public class Gyroscope : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Quaternion grot = Quaternion.Euler (Arduino.gyroangle);
		transform.rotation = grot;
	}
}
