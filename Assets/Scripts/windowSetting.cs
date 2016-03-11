using UnityEngine;
using System.Collections;

public class windowSetting : MonoBehaviour {
	Rect newWindow;
	string a;
	// Use this for initialization
	void Start () {
		newWindow = new Rect (0,0,100,100);
	}
	void OnGUI(){
		newWindow=GUI.Window(1,newWindow,WindowFunc,"Setting");
	}
	void WindowFunc(int windowID){
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
