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
		get { return new Vector3(a.x, a.z, a.y); }
	}
	public static Vector3 gyroscope // degrees per second
	{
		get { return new Vector3(-g.x, -g.z, -g.y); }
	}
	public static Vector3 gyroangle // degrees
	{
		get {
			if (gSum.x >= 0 && gSum.x < 360 && gSum.y >= 0 && gSum.y < 360 && gSum.z >= 0 && gSum.z < 360)
				return gSum;
			Vector3 ng = gSum;
			Vector3 scale = ng / 360;
			scale.x = scale.x - Mathf.Floor (scale.x);
			scale.y = scale.y - Mathf.Floor (scale.y);
			scale.z = scale.z - Mathf.Floor (scale.z);
			return gSum = scale * 360;
		}
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
			gSum += gyroscope * Time.deltaTime;
		}
		catch {
			Debug.Log ("Error in parsing");
		}
	}

}
