using UnityEngine;
using System.Collections;
using System.IO;
public class windowSetting : MonoBehaviour {
	private Rect newWindow;
 	private float Timer=0f;
 	private bool shortMode=false;
	private bool midleMode=true;
	private bool longMode=false;
	private string timeMode;
	private GameController GameControllerScripts;
	private bool windowEnable=false;
	private bool PhidgetDebug=false;
	private bool[] phidgetNum=new bool[6];
	private PhidgetsController phidgetScripts;
	// Use this for initialization
	void Awake(){
		DontDestroyOnLoad (this);
	}
	void Start () {
		phidgetScripts = GameObject.Find ("PhidgetObj").GetComponent<PhidgetsController> ();
		GameControllerScripts = GameObject.Find ("GameControl").GetComponent<GameController> ();
		newWindow = new Rect (0,0,Screen.width*0.8f,Screen.height*0.8f);
		if (PlayerPrefs.HasKey("mode")) {
			timeMode = PlayerPrefs.GetString("mode");
			setTimeMode(timeMode);
		}

	}
	void OnGUI(){
		if(windowEnable)
			newWindow=GUI.Window(1,newWindow,WindowFunc,"GameSetting");
	}
	void WindowFunc(int windowID){
		//制限時間の設定
		GUI.Label (new Rect (30, 20, 200, 30),"Time Limit "+Timer+"sec mode");
		shortMode=GUI.Toggle(new Rect(30,40,50,30),shortMode,"30sec");
		midleMode=GUI.Toggle(new Rect(90,40,50,30),midleMode,"60sec");
		longMode=GUI.Toggle(new Rect(150,40,50,30),longMode,"90sec");
		changeMode ();
		GameControllerScripts.setTimer (Timer);
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
			Timer=30f;
			break;
		case "middle":
			shortMode=false;
			midleMode=true;
			longMode=false;
			Timer=60f;
		break;
		case "long":
			shortMode=false;
			midleMode=false;
			longMode=true;
			Timer=90f;
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

	void OnApplicationQuit()//終了時処理
	{
	}
}
