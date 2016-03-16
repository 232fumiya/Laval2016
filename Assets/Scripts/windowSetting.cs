using UnityEngine;
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
	private string langStr="French";
	private TutorialScript tutorial;
	private float AddSize=0.06f;
	// Use this for initialization
	void Awake(){
		DontDestroyOnLoad (this);
	}
	void Start () {
		GameControllerScripts = GameObject.Find ("GameControl").GetComponent<GameController> ();
		phidget = GameObject.Find ("PhidgetObj").GetComponent<PhidgetsController> ();
		float WinSize = 350f;
		newWindow = new Rect (Screen.width/2-WinSize/2,Screen.height/2-WinSize/2,WinSize,WinSize);
		if (PlayerPrefs.HasKey("mode")) {
			timeMode = PlayerPrefs.GetString("mode");
			setTimeMode(timeMode);
		}
		if (PlayerPrefs.HasKey ("lang")) {
			langStr = PlayerPrefs.GetString("lang");
			setLang(langStr);
		}
		if (PlayerPrefs.HasKey ("AddSize")) {
			AddSize = PlayerPrefs.GetFloat("AddSize");
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
		GameControllerScripts.setAddSize (AddSize);
		changeLang ();
		if (tutorial != null) {
			tutorial.stLang(JA,EN,FR);
		}
		GameControllerScripts.setLng (langStr);
		}
	void WindowFunc(int windowID){
		//制限時間の設定
		GUI.Label (new Rect (30, 20, 200, 30),"Time Limit "+Timer+"sec mode");
		shortMode=GUI.Toggle(new Rect(30,50,50,30),shortMode,shortTime+"sec");
		midleMode=GUI.Toggle(new Rect(130,50,50,30),midleMode,middleTime+"sec");
		longMode=GUI.Toggle(new Rect(230,50,50,30),longMode,longTime+"sec");
		DebugMode=GUI.Toggle(new Rect(30,80,300,30),DebugMode,"DebugMode:if click N key = Next Scene");
		MirrorDebugMode=GUI.Toggle(new Rect(30,110,300,30),MirrorDebugMode,"Player is Enemy Mode");
		IsLogView=GUI.Toggle(new Rect(30,140,300,30),IsLogView,"Log View Mode");
		isSafetyPhidget = GUI.Toggle (new Rect (30, 170, 200, 30), isSafetyPhidget, "Phidgets Safety Mode");
		skipTutorial =GUI.Toggle(new Rect(30,200,200,30),skipTutorial,"skipTutorialMode");
		GUI.Label (new Rect (30, 230, 200, 30),"Language:"+langStr);
		JA=GUI.Toggle(new Rect(30,260,100,30),JA,"Japanese");
		EN=GUI.Toggle(new Rect(130,260,100,30),EN,"English");
		FR=GUI.Toggle(new Rect(230,260,100,30),FR,"French");
		AddSize = GUI.HorizontalSlider (new Rect (30, 320, 100, 30), AddSize, 0.05f, 0.2f);
		AddSize = Mathf.Floor (AddSize*100);
		AddSize = AddSize / 100;
		GUI.Label (new Rect (30, 290, 200, 20),"SnowAddSize:"+AddSize);
	}

	void changeLang(){
		if (JA && langStr !="Japanese") {
			langStr="Japanese";
		} else if (EN && langStr !="English") {
			langStr="English";
		} else if (FR &&langStr !="French") {
			langStr="French";
		}
		setLang(langStr);

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
	private void setLang(string lang)
	{
		switch (lang) {
		case "Japanese":
			FR = false;
			EN = false;
			JA=true;
			break;
		case "French":
			JA=false;
			EN=false;
			FR=true;
			break;
		case "English":
			JA = false;
			FR = false;
			EN=true;
			break;
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
		PlayerPrefs.SetString ("lang",langStr);
		PlayerPrefs.SetFloat ("AddSize",AddSize);

	}
}
