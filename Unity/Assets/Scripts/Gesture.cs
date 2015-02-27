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
		Vector3 up = Arduino.filteredAccelerometer;
		float sign = (up.x < 0 ? -1 : 1);
		return sign * Vector3.Angle (Vector3.up, new Vector3 (up.x, up.y, 0));
	}

	public static float PitchAngle() {
		Vector3 up = Arduino.filteredAccelerometer;
		float sign = (up.z < 0 ? -1 : 1);
		return sign * Vector3.Angle (Vector3.up, new Vector3 (0, up.y, up.z));
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Jumping = " + IsJumping () + " ::: Sliding = " + IsSliding());
	}
}
