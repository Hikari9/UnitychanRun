using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class Arduino : MonoBehaviour {

	// static string CommPort = SerialPort.GetPortNames()[0];
	static int BaudRate = 9600;

	public static Vector3 accelerometer
	{
		get {
			if (!connected) return Vector3.up * 256;
			return new Vector3(-a.x, a.z, -a.y);
		}
	}


	public static float gravity
	{

		get { return accelerometer.magnitude; }
	}


	public static Quaternion accelQuaternion
	{

		get { return Quaternion.FromToRotation(Vector3.up, accelerometer); }
	}


	public static Vector3 accelEuler
	{

		get { return accelQuaternion.eulerAngles; }
	}


	public static Vector3 gyroscope // degrees per second
	{

		get {
			if (!connected) return Vector3.zero;
			return new Vector3(g.x, g.z, g.y);
		}
	}


	public static Vector3 filteredAccelerometer // degrees
	{

		get {
			if (!connected) return Vector3.up * 256;
			return look * accelerometer.magnitude;
		}
	}


	public static Quaternion filteredAccelQuaternion
	{

		get {
			return Quaternion.FromToRotation (Vector3.up, filteredAccelerometer);
		}
	}


	public static Vector3 filteredAccelEuler
	{

		get { return filteredAccelQuaternion.eulerAngles; }
	}

	static Vector3 a, g;
	static Vector3 look;
	static SerialPort Serial;
	const int Bytes = 22;
	static byte[] buffer = new byte[Bytes];
	static int ptr = 0;

	static bool connected = false;
	static bool preparing = false;

	static void OnException(Exception ex) {
		Debug.LogException (ex);
		connected = false;
		if (Serial != null)
			Serial.Close ();
	}

	static void Prepare() {
		preparing = true;
		try {
			Debug.Log ("Getting ports...");
			string[] ports = SerialPort.GetPortNames();

			if (ports == null || ports.Length == 0) {
				throw new IOException("No ports available");
			}

			Debug.Log ("Creating Serial...");
			Serial = new SerialPort();

			
			Serial.PortName = ports[0];
			Serial.BaudRate = BaudRate;
			Serial.Parity = Parity.None;
			Serial.StopBits = StopBits.One;
			Serial.ReadTimeout = BaudRate;

			Debug.Log ("Checking if serial is open...");
			if (Serial.IsOpen) {
				Debug.Log ("Serial indeed open. Closing before proceeding...");
				Serial.Close ();
			}

			Debug.Log ("Opening Serial...");
			Serial.Open ();

			Debug.Log ("Reading initial data...");

			Read ();
			Parse ();
			look = accelerometer.normalized;
			ComplementaryFilter ();
			connected = true;

			Debug.Log ("Connected!");
		}
		catch (Exception ex) {
			OnException (ex);
		}
		preparing = false;
	}

	// Update is called once per frame
	void Update () {
		if (preparing) return;
		if (!connected) {
			Prepare ();
		}
		else {
			try {
				Read ();
				Parse ();
				ComplementaryFilter ();
			}
			catch (Exception ex) {
				OnException (ex);
			}
		}
	}

	static void Read() {
		int Threshold = Bytes;
		while (Threshold-- > 0) {
			int r = Serial.ReadByte ();
			if (r < 0)
				throw new IOException("No byte to read: " + r);
			buffer[ptr++] = (byte) r;
			if (ptr == buffer.Length) ptr = 0;
		}
	}

	static void Parse() {
		MemoryStream ms = new MemoryStream(buffer);
		BinaryReader br = new BinaryReader(ms);
		a = new Vector3(br.ReadInt16 (), br.ReadInt16 (), br.ReadInt16 ());
		g = new Vector3(br.ReadSingle (), br.ReadSingle (), br.ReadSingle());
	}

	
	static float trustGyro = 5f;

	static void ComplementaryFilter() {
		Quaternion delta = Quaternion.Euler (gyroscope * Time.deltaTime);
		Vector3 gLook = delta * look;
		look = (accelerometer.normalized + gLook * trustGyro) / (1 + trustGyro);
	}

	static float idleMag = 10; // degrees per second
	static float idleTimeAllowance = 0.17f; // seconds
	static float firstIdleTime = 0;
	public static bool isIdle = false;

	static bool IsIdle() {
		if (gyroscope.x < idleMag && gyroscope.y < idleMag && gyroscope.z < idleMag) {
			// trigger idle
			firstIdleTime = Time.time;
			isIdle = true;
		} else if (Time.time - firstIdleTime >= idleTimeAllowance) {
			// not idle anymore
			isIdle = false;
		}
		return isIdle;
	}


	void OnDestroy() {
		if (Serial != null)
			Serial.Close ();
	}

	void OnApplicationQuit() {
		if (Serial != null)
			Serial.Close ();
	}

	public static void Close() {
		connected = false;
		if (Serial != null)
			Serial.Close ();
	}

}
