using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class Arduino : MonoBehaviour {
	
	static string CommPort = "COM1";
	static int BaudRate = 9600;

	public static Vector3 accelerometer
	{
		get { return a; }
	}
	public static Vector3 gyroscope // degrees per second
	{
		get { return g; }
	}
	public static Vector3 gyroangle // degrees
	{
		get { return gSum; }
	}

	static Vector3 a = Vector3.zero, g = Vector3.zero;
	static Vector3 gSum = Vector3.zero;
	static SerialPort Serial;
	const int Bytes = 22;
	static byte[] buffer = new byte[Bytes];
	static int ptr = 0;

	// Use this for initialization
	void Start () {
		Serial = new SerialPort (CommPort, BaudRate);
		Serial.Open ();
	}

	// Update is called once per frame
	void Update () {
		// new Thread (Read).Start ();
		Read ();
		Parse ();
		// Parse ();
	}

	void Read() {
		if (Serial == null || !Serial.IsOpen)
			return;
		int Threshold = Bytes;
		while (Threshold-- > 0) {
			buffer[ptr++] = (byte) Serial.ReadByte();
			if (ptr == buffer.Length) ptr = 0;
		}
	}

	void Parse() {
		if (Serial == null || !Serial.IsOpen)
			return;
		Read ();
		try {
			MemoryStream ms = new MemoryStream(buffer);
			BinaryReader br = new BinaryReader(ms);
			a = new Vector3(br.ReadInt16 (), br.ReadInt16 (), br.ReadInt16 ());
			g = new Vector3(br.ReadSingle (), br.ReadSingle (), br.ReadSingle());
			gSum += g * Time.deltaTime;
		}
		catch {
			Debug.Log ("Error in parsing");
		}
	}

}
