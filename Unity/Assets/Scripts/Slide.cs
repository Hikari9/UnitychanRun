using UnityEngine;
using System.Collections;

public class Slide : MonoBehaviour {

	public float slideTime = 3f;
	[Range(0f, 0.5f)]
	public float fadeTime = 0.1f; // ratio
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

		// set animation to sliding
		if (slideAnimation != null) {
			con.runAnimation = slideAnimation;
		}
		con.canJump = false;


		float startTime = Time.time;
		float fade = fadeTime * slideTime;

		while (Time.time - startTime < fade) {
			float diff = Time.time - startTime;
			transform.localRotation = Quaternion.Slerp (Quaternion.identity, slideRotation, diff / fade);
			yield return new WaitForEndOfFrame();
		}
		transform.localRotation = slideRotation;
		yield return new WaitForSeconds (slideTime - fade * 2);
		startTime += slideTime - fade;
		while (Time.time - startTime < fade) {
			float diff = Time.time - startTime;
			transform.localRotation = Quaternion.Slerp (slideRotation, Quaternion.identity, diff / fade);
			yield return new WaitForEndOfFrame();
		}
		transform.localRotation = Quaternion.identity;

		// return run animation

		if (slideAnimation != null) {
			con.runAnimation = prevAnimation;
		}
		
		con.canJump = true;
		slideLock = false;
	}

}
