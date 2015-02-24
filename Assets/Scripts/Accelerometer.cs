using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

public class Accelerometer : MonoBehaviour {

	string CommPort = "COM5";
	int BaudRate = 9400;
	public float strength = 0.02f;
	string buffer;
	string[] sp;
	
	SerialPort Serial;
	Transform cam;
	
	// Use this for initialization
	void Start () {
		Serial = new SerialPort (CommPort, BaudRate);
		cam = GameObject.FindGameObjectWithTag ("MainCamera").transform;
		Serial.Open ();
	}

	// Update is called once per frame
	void Update () {
		int check = 0;
		try {
			while (check++ < 10)
				buffer = Serial.ReadLine ();
			Vector3 dir = cam.position - transform.position;
			Quaternion face = Quaternion.LookRotation (dir);
			sp = buffer.Split (' ');
			int x = int.Parse (sp [0]);
			int y = int.Parse (sp [1]);
			int z = int.Parse (sp [2]);
			Vector3 accel = new Vector3 (x, y, z);
			// transform.position += accel * strength * Time.deltaTime;
			Quaternion nr = Quaternion.LookRotation (accel);
			// transform.rotation = Quaternion.Slerp (transform.rotation, face * nr, Mathf.Min (1f, 2f * Time.deltaTime));
			transform.rotation = nr * face;
		}
		catch { }
	}

}
