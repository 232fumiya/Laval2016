﻿using UnityEngine;
using System.Collections;
using System.IO;
public class windowSetting : MonoBehaviour {
	private Rect newWindow;
 	private float Timer=0f;
 	private bool shortMode=false;
	private bool midleMode=true;
	private bool longMode=false;
	private float shortTime=30f,middleTime=60f,longTime=90f;
	private string timeMode;
	private GameController GameControllerScripts;
	private bool windowEnable=false;
	private bool DebugMode=false;
	private bool MirrorDebugMode=false;
	private bool IsLogView=false;
	private bool isSafetyPhidget=false;
	private PhidgetsController phidget;
	private bool skipTutorial=false;
	private bool FR=true;
	private bool JA=false;
	private bool EN=false;
	private string beforeLang="French";
	private TutorialScript tutorial;
	// Use this for initialization
	void Awake(){
		DontDestroyOnLoad (this);
	}
	void Start () {
		GameControllerScripts = GameObject.Find ("GameControl").GetComponent<GameController> ();
		phidget = GameObject.Find ("PhidgetObj").GetComponent<PhidgetsController> ();
		newWindow = new Rect (0,0,Screen.width*0.8f,Screen.height*0.8f);
		if (PlayerPrefs.HasKey("mode")) {
			timeMode = PlayerPrefs.GetString("mode");
			setTimeMode(timeMode);
		}
		GameControllerScripts.setTimer (Timer);
	}
	void OnGUI(){
		if (windowEnable) {
			Cursor.visible = true;
			newWindow = GUI.Window (1, newWindow, WindowFunc, "GameSetting");
		} else {
			Cursor.visible = false;
		}
	}
	void Update(){
		if (tutorial == null && Application.loadedLevelName=="Tutorial") {
			tutorial=GameObject.Find("Player").GetComponent<TutorialScript>();
		}
		changeMode ();
		GameControllerScripts.getDebugMode (DebugMode);
		GameControllerScripts.setMirrorMode (MirrorDebugMode);
		GameControllerScripts.isViewLog (IsLogView);
		phidget.setSafety (isSafetyPhidget);
		GameControllerScripts.setTimer (Timer);
		GameControllerScripts.setSkipTutorialMode (skipTutorial);
		changeLang ();
		if (tutorial != null) {
			tutorial.stLang(JA,EN,FR);
		}
		}
	void WindowFunc(int windowID){
		//制限時間の設定
		GUI.Label (new Rect (30, 20, 200, 30),"Time Limit "+Timer+"sec mode");
		shortMode=GUI.Toggle(new Rect(30,60,50,30),shortMode,shortTime+"sec");
		midleMode=GUI.Toggle(new Rect(130,60,50,30),midleMode,middleTime+"sec");
		longMode=GUI.Toggle(new Rect(230,60,50,30),longMode,longTime+"sec");
		DebugMode=GUI.Toggle(new Rect(30,100,300,30),DebugMode,"DebugMode:if click N key = Next Scene");
		MirrorDebugMode=GUI.Toggle(new Rect(30,140,300,30),MirrorDebugMode,"Player is Enemy Mode");
		IsLogView=GUI.Toggle(new Rect(30,180,300,30),IsLogView,"Log View Mode");
		isSafetyPhidget = GUI.Toggle (new Rect (30, 220, 200, 30), isSafetyPhidget, "Phidgets Safety Mode");
		skipTutorial =GUI.Toggle(new Rect(30,260,200,30),skipTutorial,"skipTutorialMode");
		GUI.Label (new Rect (30, 300, 200, 30),"Language:"+beforeLang);
		JA=GUI.Toggle(new Rect(30,320,100,30),JA,"Japanese");
		EN=GUI.Toggle(new Rect(130,320,100,30),EN,"English");
		FR=GUI.Toggle(new Rect(230,320,100,30),FR,"French");
	}

	void changeLang(){
		if (JA && beforeLang !="Japanese") {
			FR = false;
			EN = false;
			JA=true;
			beforeLang="Japanese";
		} else if (EN && beforeLang !="English") {
			JA = false;
			FR = false;
			EN=true;
			beforeLang="English";
		} else if (FR &&beforeLang !="French") {
			JA=false;
			EN=false;
			FR=true;
			beforeLang="French";
		}

	}

	void changeMode(){
		if (shortMode&&Timer!=30) {
			setTimeMode ("short");
		} else if (midleMode&&Timer!=60) {
			setTimeMode ("middle");
		} else if (longMode&&Timer!=90) {
			setTimeMode("long");
		}
	}
	private void setTimeMode(string trueMode)
	{
		switch (trueMode) {
		case "short":
			shortMode=true;
			midleMode=false;
			longMode=false;
			Timer=shortTime;
			break;
		case "middle":
			shortMode=false;
			midleMode=true;
			longMode=false;
			Timer=middleTime;
		break;
		case "long":
			shortMode=false;
			midleMode=false;
			longMode=true;
			Timer=longTime;
			break;
		}
		PlayerPrefs.SetString ("mode",trueMode);
	}
	public void enableWindow()
	{
		if (windowEnable)
			windowEnable = false;
		else
			windowEnable = true;
	}
	public void setSafety(){
		if (isSafetyPhidget)
			isSafetyPhidget = false;
		else
			isSafetyPhidget = true;
	}

	void OnApplicationQuit()//終了時処理
	{
	}
}
