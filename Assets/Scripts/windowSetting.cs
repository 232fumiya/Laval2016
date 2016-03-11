using UnityEngine;
using System.Collections;

public class windowSetting : MonoBehaviour {
	private Rect newWindow;
 	private float Timer=60f;
 	private bool shortMode=false;
	private bool midleMode=true;
	private bool longMode=false;
	private GameController GameControllerScripts;
	// Use this for initialization
	void Awake(){
		DontDestroyOnLoad (this);
	}
	void Start () {
		GameControllerScripts = GameObject.Find ("GameControl").GetComponent<GameController> ();
		Timer = GameControllerScripts.getTimer ();
		newWindow = new Rect (0,0,Screen.width/2,Screen.height/2);
	}
	void OnGUI(){
		newWindow=GUI.Window(1,newWindow,WindowFunc,"Setting");
	}
	void WindowFunc(int windowID){
		GUI.Label (new Rect (30, 20, 200, 30),"Time Limit "+Timer+"sec mode");
		shortMode=GUI.Toggle(new Rect(30,40,50,30),shortMode,"30sec");
		midleMode=GUI.Toggle(new Rect(90,40,50,30),midleMode,"60sec");
		longMode=GUI.Toggle(new Rect(150,40,50,30),longMode,"90sec");
		changeMode ();
		GameControllerScripts.setTimer (Timer);
		GUI.DragWindow ();
	}
	void changeMode(){
		if (shortMode&&Timer!=30) {
			midleMode = false;
			longMode = false;
			Timer = 30f;
		} else if (midleMode&&Timer!=60) {
			shortMode = false;
			longMode = false;
			Timer = 60f;
		} else if (longMode&&Timer!=90) {
			shortMode=false;
			midleMode=false;
			Timer=90f;
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
