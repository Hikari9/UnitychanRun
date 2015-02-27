using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gesture : MonoBehaviour {

	public static bool LeftTurn() {

		return false;
	}

	public static bool RightTurn() {
		
		return false;
	}

	static bool slideLock = false;
	static float slideAngle = -58; // degrees

	public static bool Slide() { // backward pitch
		if (PitchAngle () < slideAngle) {
			if (slideLock)
				return false;
			return slideLock = true;
		}
		return slideLock = false;
	}

	public static bool IsSliding() {
		return Gesture.Slide () || slideLock;
	}

	static float jumpAngle = 45; // accel should point up within range
	static float jumpMag = 120; // when jumping up, accel magnitude approaches 0
	static bool jumpLock = false;

	public static bool Jump() {
		Vector3 up = Arduino.filteredAccelerometer;
		float angle = Vector3.Angle (Vector3.up, up);
		if (angle < jumpAngle && up.magnitude < jumpMag) {
			if (jumpLock)
				return false;
			return jumpLock = true;
		}
		return jumpLock = false;
	}

	public static bool IsJumping() {
		return Gesture.Jump () || jumpLock;
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
