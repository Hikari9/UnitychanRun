using UnityEngine;
using System.Collections;

public class Orbiting : MonoBehaviour {

	public string[] commands = new string[]{
		"w", "a", "s", "d", "up", "down"
	};

	public GameObject target;

	int[] _dx = {0, -1, 0, 1, 0, 0};
	int[] _dy = {1, 0, -1, 0, 0, 0};
	int[] _dz = {0, 0, 0, 0, 1, -1};

	public float maxVelocity = 10f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		FaceObject ();
		ReceiveInput ();
	}

	void FaceObject() {
		if (target == null) return;
		Vector3 delta = target.transform.position - transform.position;
		Quaternion targetRotation = Quaternion.LookRotation (delta);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Mathf.Min (Time.deltaTime * 10f, 1f));
	}

	void ReceiveInput() {
		Vector3 velocity = Vector3.zero;
		for (int i = 0; i < commands.Length; ++i) {
			if (Input.GetKey (commands[i])) {
				velocity += new Vector3(_dx[i], _dy[i], _dz[i]);
			}
		}
		velocity.Normalize ();
		transform.position += transform.rotation * velocity * maxVelocity * Time.deltaTime;
	}
}
