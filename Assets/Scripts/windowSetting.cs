using UnityEngine;
using System.Collections;

public class windowSetting : MonoBehaviour {
	private Rect newWindow;
 	private float Timer=60f;
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
		GUI.Label (new Rect (30, 20, 100, 30), "Time Limit");
		Timer=float.Parse(GUI.TextField(new Rect(30,40,100,30),Timer.ToString()));
		GameControllerScripts.setTimer (Timer);
		GUI.DragWindow ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
