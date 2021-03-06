﻿using UnityEngine;
using System.Collections;

public abstract class VJAbstractManager : MonoBehaviour {

	// Debug Window	properties
	protected static int windowIDSeed = 3000;
	protected static float maxBarLength = 90.0f;

    protected Rect windowRect = new Rect(160, 20, 120, 50);
	protected Texture2D _debugSpectrumTex;
	protected int windowId;
	public bool debugWindow = false;
	
//	protected abstract void OnAwake();
//	protected abstract void OnStart();
//	protected abstract void OnUpdate();
//	protected abstract void OnDrawGUI();

	public virtual void Awake() {
		_debugSpectrumTex = new Texture2D(16,16);

		windowRect.x = PlayerPrefs.GetFloat("vjkit.window.pos." + gameObject.name + ".x", Screen.width - 220.0f);
		windowRect.y = PlayerPrefs.GetFloat("vjkit.window.pos." + gameObject.name + ".y", 20.0f);

		windowRect.width = 200;
		windowId = windowIDSeed;
		windowIDSeed++;
	}
	
	public static VJAbstractManager GetDefaultManager() {
  		VJAbstractManager[] managers = UnityEngine.Object.FindObjectsOfType(typeof(VJAbstractManager)) as VJAbstractManager[];
  		if( managers.Length > 0 ) {
  			return managers[0];
  		}
  		return null;
  	}
	
	public VJAbstractDataSource GetDefaultDataSource() {
		VJAbstractDataSource[] sources = gameObject.GetComponents<VJAbstractDataSource>() as VJAbstractDataSource[];
		if( sources.Length > 0 ) {
			return sources[0];
		}
		return null;
	}
	

    protected virtual void OnDrawGUIWindow(int windowID) {
		GUILayout.BeginVertical();

		VJAbstractDataSource[] sources = gameObject.GetComponents<VJAbstractDataSource>() as VJAbstractDataSource[];

		foreach(VJAbstractDataSource src in sources) {
			GUILayout.Label(src.sourceName + ":");
			GUILayout.Space(8.0f);
			Rect r = GUILayoutUtility.GetLastRect();
			Rect zeroBar = r;
			//r.y+=16;
			r.height = 4;
			r.x += (int)maxBarLength;
			r.width = Mathf.Clamp(180.0f * src.current /VJAbstractDataSource.s_prog_max, -maxBarLength, maxBarLength);
			Color lastColor = GUI.contentColor;
			GUI.color = Color.red;
			GUI.DrawTexture(r, _debugSpectrumTex, ScaleMode.StretchToFill, false, 0.0f);
			r.y+=4;
			r.width = Mathf.Clamp(180.0f * src.previous /VJAbstractDataSource.s_prog_max, -maxBarLength, maxBarLength);
			GUI.color = Color.blue;
			GUI.DrawTexture(r, _debugSpectrumTex, ScaleMode.StretchToFill, false, 0.0f);
			r.y+=4;
			r.width = Mathf.Clamp(180.0f * src.diff /VJAbstractDataSource.s_prog_max, -maxBarLength, maxBarLength);
			GUI.color = Color.yellow;
			GUI.DrawTexture(r, _debugSpectrumTex, ScaleMode.StretchToFill, false, 0.0f);

			GUI.color = Color.white;
			zeroBar.height = 12;
			zeroBar.width = 1;
			zeroBar.x += (int)maxBarLength;
			GUI.DrawTexture(zeroBar, _debugSpectrumTex, ScaleMode.StretchToFill, false, 0.0f);

			GUI.color = lastColor;
			GUI.DragWindow ();
		}				
		GUILayout.EndVertical();
    }
    
	public virtual void OnGUI() {
		if( debugWindow ) {
			windowRect = GUILayout.Window(windowId, windowRect, OnDrawGUIWindow, name, GUILayout.Width(200));
		}
	}
	
	public virtual void OnApplicationQuit() {
		PlayerPrefs.SetFloat("vjkit.window.pos." + gameObject.name + ".x", windowRect.x);
		PlayerPrefs.SetFloat("vjkit.window.pos." + gameObject.name + ".y", windowRect.y);
	}
}
