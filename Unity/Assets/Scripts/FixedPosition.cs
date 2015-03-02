using UnityEngine;
using System.Collections;

public class FixedPosition : MonoBehaviour {

	public bool freezeX = false, freezeY = false, freezeZ = false;
	Vector3 original;

	void Start () {
		original = transform.position;
	}

	void FixedUpdate () {
		if (freezeX) {
			transform.position = new Vector3(original.x, transform.position.y, transform.position.z);
		}
		if (freezeY) {
			transform.position = new Vector3(transform.position.x, original.y, transform.position.z);
		}
		if (freezeZ) {
			transform.position = new Vector3(transform.position.x, transform.position.y, original.z);
		}
	}
}
