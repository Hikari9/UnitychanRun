using UnityEngine;
using System.Collections;
using System;

public class HUD : MonoBehaviour {

	float currTime;
	GUIStyle style;
	bool gameHasEnded;

	void OnGUI() {
		if (gameHasEnded)
			DisplayFinal ();
		else
			currTime = Time.timeSinceLevelLoad;
		style.alignment = TextAnchor.MiddleLeft;
		GUI.Label (new Rect (25, 25, 200, 50), ("Score: "+ Math.Round (currTime*1000)), style);
	}

	void Start() {
		style = new GUIStyle ();
		style.fontSize = 50;
		style.normal.textColor = Color.white;
		gameHasEnded = false;
	}

	public void End() {
		gameHasEnded = true;
	}

	void Update() {
		//float newTime = (currTime - startTime);
		Debug.Log ("Time: " + currTime);
	}

	void DisplayFinal() {
		
		style.alignment = TextAnchor.MiddleCenter;
		GUI.Label (new Rect (Screen.width / 2, Screen.height / 3, 100, 100), "Game Over!", style);
		GUI.Label (new Rect (Screen.width / 2, Screen.height / 2, 100, 100), "Score: " + Math.Round (currTime * 1000), style);
	}

}