using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gesture : MonoBehaviour {

	// public static bool LeftTurn() {

		// return false;
	// }

	// public static bool RightTurn() {
		
		// return false;
	// }

	static float slideAngle = -58; // degrees
	static float slideAnglePerSecond = 180; // degrees per second

	public static bool IsSliding() { // backward pitch=
		return -Arduino.gyroscope.x >= slideAnglePerSecond && PitchAngle () < slideAngle;
	}

	static float jumpAngle = 45; // accel should point up within range
	static float jumpMag = 120; // when jumping up, accel magnitude approaches 0

	public static bool IsJumping() {
		Vector3 up = Arduino.filteredAccelerometer;
		float angle = Vector3.Angle (Vector3.up, up);
		return (angle < jumpAngle && up.magnitude < jumpMag);
	}

	public static float RollAngle() {
		float roll = Arduino.filteredAccelEuler.z;
		if (roll > 180) roll -= 360;
		return roll;
	}

	public static float PitchAngle() {
		float pitch = Arduino.filteredAccelEuler.x;
		if (pitch > 180) pitch -= 380;
		return pitch;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
