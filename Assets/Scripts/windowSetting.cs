using UnityEngine;
using System.Collections;
using System.IO;
public class windowSetting : MonoBehaviour {
	private Rect newWindow;
 	private float Timer=0f;
 	private bool shortMode=false;
	private bool midleMode=false;
	private bool longMode=false;
	private GameController GameControllerScripts;
	private TextAsset SettingFile;
	// Use this for initialization
	void Awake(){
		DontDestroyOnLoad (this);
	}
	void Start () {
		GameControllerScripts = GameObject.Find ("GameControl").GetComponent<GameController> ();
		newWindow = new Rect (0,0,Screen.width/2,Screen.height/2);
		SettingFile = Resources.Load ("datafile")as TextAsset;
		readCSV ();
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
			setTimeMode ("short");
		} else if (midleMode&&Timer!=60) {
			setTimeMode ("middle");
		} else if (longMode&&Timer!=90) {
			setTimeMode("long");
		}
	}
	void readCSV(){
		StringReader reader = new StringReader (SettingFile.text);
		//Read Header
		string header = reader.ReadLine ();
		string[] headersValue = header.Split (',');
		//Read Body
		string body = reader.ReadLine ();
		string[] values = body.Split (',');
		
		for (int i=0; i<headersValue.Length; i++) {
			switch(headersValue[i].ToString())
			{
			case "GameTime":
				Debug.Log(values[i].ToString());	
				switch(values[i].ToString())
					{
					case "30":
						setTimeMode ("short");
						break;
					case "60":
						setTimeMode ("middle");
						break;
					case "90":
						setTimeMode("long");
						break;
					}
				break;
			}
		}
		reader.Close ();
	}
	void writeCSV(){
		string filePath = Application.dataPath + "/Resources/datafile.csv";
		string[] lines =System.IO.File.ReadAllLines (filePath);
		lines.SetValue(Timer.ToString(),1);
		System.IO.File.WriteAllLines(filePath, lines);
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
		writeCSV ();
	}

	void OnApplicationQuit()//終了時処理
	{
		writeCSV ();
	}
}
