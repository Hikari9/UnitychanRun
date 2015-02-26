using UnityEngine;
using System.Collections;

public class Gyroscope : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	
	// Update is called once per frame
	void Update () {

		// rotate by small omega
		Quaternion next = Quaternion.Euler (Arduino.gyroscope * Time.deltaTime);
		transform.rotation *= next;

		Vector3 gx, gy, gz;
		gx = transform.right;
		gy = transform.up;
		gz = transform.forward;

		transform.up = Arduino.filteredAccelerometer;

		Vector3 ax, ay, az;
		ax = transform.right;
		ay = transform.up;
		az = transform.forward;

		// to do: up = accel, right = project(gx to ax), forward = project (gz to az)
		Vector3 up = ay;
		Vector3 right = gx - up * Vector3.Dot (up, gx);
		Vector3 forward = gz - up * Vector3.Dot (up, gz);

		up.Normalize ();
		right.Normalize ();
		forward.Normalize ();

		transform.rotation = Quaternion.LookRotation (forward, up);

	}
}
