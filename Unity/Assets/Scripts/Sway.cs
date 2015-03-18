using UnityEngine;
using System.Collections;

public class Sway : MonoBehaviour {
	
	public float inclination = 75f;
	public float maxDistance = 10f;
	public float dampness = 0.5f; // smoothen sway
	
	float prevAngle = 0;
	public float currentInclination {
		get { 
			Vector3 projected = Vector3.ProjectOnPlane (Arduino.filteredAccelerometer.normalized, Vector3.forward);
			if (projected.magnitude < 0.3f || Arduino.filteredAccelerometer.y < 0) return prevAngle;

			float angle = -Gesture.RollAngle ();
			angle = Mathf.Max (-inclination, Mathf.Min (inclination, angle));
			return prevAngle = angle;
		}
	}

	public float displacement {
		get {
			return (currentInclination / inclination * maxDistance);
		}
	}

	void Start() {

	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.Lerp (transform.localPosition, transform.localRotation * Vector3.right * displacement, Time.deltaTime / dampness);
	}
}
