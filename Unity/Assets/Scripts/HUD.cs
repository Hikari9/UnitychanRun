using UnityEngine;
using System.Collections;
using System;

public class HUD : MonoBehaviour {

	float currTime;
	GUIStyle style;

	void OnGUI() {
		style = new GUIStyle ();
		style.fontSize = 50;
		style.normal.textColor = Color.white;
		currTime = Time.timeSinceLevelLoad;
		GUI.Label (new Rect (25, 25, 200, 50), ("Score: "+ Math.Round (currTime*1000)), style);
	}

	void Update() {
		currTime = Time.timeSinceLevelLoad;
		//float newTime = (currTime - startTime);
		Debug.Log ("Time: " + currTime);
		GUI.Label (new Rect (25, 25, 200, 50), ("Score: "+ Math.Round (currTime*1000)), style);
	}

	void DisplayFinal() {
		GUI.Label (new Rect (Screen.width / 4, Screen.height / 4, 100, 100), "Game Over!", style);
		GUI.Label (new Rect (Screen.width / 4, Screen.height / 3, 100, 100), "Score: " + Math.Round (currTime * 1000), style);
	}

}