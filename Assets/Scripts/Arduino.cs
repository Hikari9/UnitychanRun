using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class Arduino : MonoBehaviour {

	public string CommPort = "COM5";
	public int BaudRate = 9600;
	public byte[] buf = new byte[3 * 2];

	SerialPort Serial;

	// Use this for initialization
	void Start () {
		Serial = new SerialPort (CommPort, BaudRate);
		Serial.Open ();
	}
	
	// Update is called once per frame
	void Update () { 
		if (!Input.GetKey ("space")) return;
		Debug.Log (Serial.ReadLine ());
	}

	int[] BytesToShorts(byte[] b) {
		int[] s = new int[b.Length / 2];
		for (int i = 0; i < b.Length; i += 2) {
			s[i / 2] = ((int) b[i] << 8) | b[i + 1];
		}
		return s;
	}
}
