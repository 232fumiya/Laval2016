using UnityEngine;
using System.Collections;
using System.IO;
public class windowSetting : MonoBehaviour {
	private Rect newWindow;
 	private float Timer=60f;
 	private bool shortMode=false;
	private bool midleMode=true;
	private bool longMode=false;
	private GameController GameControllerScripts;
	private TextAsset SettingFile;
	// Use this for initialization
	void Awake(){
		DontDestroyOnLoad (this);
	}
	void Start () {
		GameControllerScripts = GameObject.Find ("GameControl").GetComponent<GameController> ();
		Timer = GameControllerScripts.getTimer ();
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
	void readCSV(){
		StringReader reader = new StringReader (SettingFile.text);
		//Read Header
		string header = reader.ReadLine ();
		string[] headersValue = header.Split (',');
		//Read Body
		string body = reader.ReadLine ();
		string[] values = body.Split (',');
		
		for (int i=0; i<header.Length; i++) {
			switch(header[i].ToString())
			{
			case "GameTime":
				Timer=float.Parse(body[i].ToString());
				break;
			}
		}
	}
	void writeCSV(){
		string filePath = Application.dataPath + "/Resources/datafile.csv";
		string[] lines = System.IO.File.ReadAllLines (filePath);
		lines.SetValue(Timer.ToString(),1);
		//最初の1行を削除するなら、次のようにする
		//lines = lines.Skip(1).ToArray();
		//テキストファイルに上書き保存する
		System.IO.File.WriteAllLines(filePath, lines);
	}
	void OnApplicationQuit()//終了時処理
	{
		writeCSV ();
	}
}
