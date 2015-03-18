using UnityEngine;
using System.Collections;

public class Slide : MonoBehaviour {

	public float slideTime = 3f;
	public AnimationClip slideAnimation = null;

	Quaternion slideRotation = Quaternion.FromToRotation (Vector3.up, Vector3.back);

	// Use this for initialization
	void Start () {

	}

	bool slideLock = false;


	bool IsSliding() {
		if (slideLock) return false;
		return !Gesture.IsJumping () && Gesture.IsSliding ();
	}


	// Update is called once per frame
	void Update () {
		if (IsSliding ()) {
			StartCoroutine(UpdateRotation ());
		}
	}

	IEnumerator UpdateRotation() {

		slideLock = true;

		
		ArduinoController con = GetComponentInParent<ArduinoController> ();
		AnimationClip prevAnimation = con.runAnimation;
		float prevAnimationSpeed = con.runMaxAnimationSpeed;

		// set animation to sliding
		if (slideAnimation != null) {
			con.runAnimation = slideAnimation;
			con.runMaxAnimationSpeed = slideAnimation.length / slideTime;
		}
		con.canJump = false;

		yield return new WaitForSeconds (slideTime);
		// return run animation

		if (slideAnimation != null) {
			con.runAnimation = prevAnimation;
			con.runMaxAnimationSpeed = prevAnimationSpeed;
		}
		
		con.canJump = true;
		slideLock = false;
	}

}
