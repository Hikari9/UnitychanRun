using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class Arduino : MonoBehaviour {
	
	static string CommPort = "COM5";
	static int BaudRate = 9600;

	public static Vector3 accelerometer
	{
		get { return a == null ? Vector3.zero : a; }
	}
	public static Vector3 gyroscope
	{
		get { return g == null ? Vector3.zero : g; }
	}

	static Vector3 a, g;
	static SerialPort Serial;
	static string buffer = "";

	// Use this for initialization
	void Start () {
		Serial = new SerialPort (CommPort, BaudRate);
		Serial.Open ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Serial == null || !Serial.IsOpen)
			return;
		new Thread (Read).Start ();
		Parse ();
	}

	void Read() {
		buffer = Serial.ReadLine ();
	}

	void Parse() {
		try {
			string currentBuffer = buffer;
			string[] sp = currentBuffer.Split (' ');
			float[] f = new float[sp.Length];
			for (int i = 0; i < sp.Length; ++i) {
				f[i] = float.Parse (sp[i]);
			}
			a = new Vector3(f[0], f[1], f[2]);
			g = new Vector3(f[3], f[4], f[5]);
		}
		catch {}
	}

}
