using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class Accelerometer : MonoBehaviour {


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Quaternion arot = Quaternion.FromToRotation (Arduino.accelerometer, Vector3.up);
		transform.rotation = arot;
	}


}
