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
		transform.up = Arduino.filteredAccelerometer;
	}


}
