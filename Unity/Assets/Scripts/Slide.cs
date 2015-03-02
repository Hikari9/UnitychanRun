using UnityEngine;
using System.Collections;

public class Slide : MonoBehaviour {

	public float slideTime = 3f;
	[Range(0f, 0.5f)]
	public float fadeTime = 0.1f; // ratio

	Quaternion slideRotation = Quaternion.FromToRotation (Vector3.up, Vector3.back);

	// Use this for initialization
	void Start () {
	
	}

	bool slideLock = false;
	float slideTimeStart = 0;

	bool IsSliding() {
		if (Gesture.IsSliding ()) {
			if (slideLock) return false;
			return slideLock = true;
		}
		return slideLock;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (Gesture.PitchAngle ());
		if (IsSliding ()) {
			// StartCoroutine(UpdateRotation());
			StartCoroutine(UpdateRotation ());
		}
	}

	IEnumerator UpdateRotation() {
		// transform.localRotation *= Quaternion.Slerp (Quaternion.identity, slideRotation, 1);
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
	}

}
