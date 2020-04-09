using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LayoutGenerator))]
public class LayoutGeneratorCustomEditor : Editor {

	// Use this for initialization
	void OnEnable () {
		
	}
	
	public override void OnInspectorGUI() {
		LayoutGenerator lg = (LayoutGenerator)target;

		base.OnInspectorGUI();

		if(GUILayout.Button("Generate Layout")) {
			// isolate the dark pixels from the screenshot
			Color[] c = lg.inputTexture.GetPixels();
			c = lg.IsolateDarkPixels(c);
			lg.inputTexture.SetPixels(0, 0, lg.resWidth, lg.resHeight, c);
			lg.inputTexture.Apply();
		}
	}
}
