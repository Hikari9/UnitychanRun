using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour {

	public float dx, dy, dz;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v = new Vector3 (dx, dy, dz) * Time.deltaTime;
		transform.Rotate (v);
	}
}
