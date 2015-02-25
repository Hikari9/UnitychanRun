using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class Accelerometer : MonoBehaviour {

	public float strength = 0.02f;
	Transform cam;

	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").transform;
	}

	// Update is called once per frame
	void Update () {
		Vector3 dir = cam.position - transform.position;
		Quaternion face = Quaternion.LookRotation (dir);
		Vector3 accel = Arduino.accelerometer;
		Vector3 gyro = Arduino.gyroangle;
		Debug.Log (accel + " " + gyro);
		// transform.position += accel * strength * Time.deltaTime;
		// Quaternion nr = Quaternion.LookRotation (accel);
		// Quaternion rot = Quaternion.Euler (gyro);
		// transform.rotation = Quaternion.Slerp (transform.rotation, face * nr, Mathf.Min (1f, 2f * Time.deltaTime));
		// transform.rotation = rot * face;
	}


}
