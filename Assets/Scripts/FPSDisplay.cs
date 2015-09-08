using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour {

	public int fontSize = 12;
	public Color color = Color.white;

	float deltaTime = 0;

	void Update () {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI() {
		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, 0, Screen.width, Screen.height);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = fontSize;
		style.normal.textColor = color;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}
}
