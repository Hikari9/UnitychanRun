using UnityEngine;
using System.Collections;

public class Sway : MonoBehaviour {
	
	public float inclination = 75f;
	public float maxDistance = 10f;
	
	float prevAngle = 0;
	public float currentInclination {
		get { 
			float angle = -Gesture.RollAngle ();
			angle = Mathf.Max (-inclination, Mathf.Min (inclination, angle));
			if (Arduino.filteredAccelerometer.y < 0 && Mathf.Sign (angle) != Mathf.Sign (prevAngle)) return prevAngle;
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
		transform.localPosition = transform.right * displacement;
	}
}
