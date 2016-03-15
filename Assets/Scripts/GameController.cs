﻿using UnityEngine;
using System.Collections;
/// <summary>
/// Game controller
/// </summary>
public class GameController : MonoBehaviour {
	private float timer;
	private float setTime=60f;
	private AudioSource dash;
	private KinectPlayer playerScript;
	private bool gameStart=false;
	private MeshRenderer oparate;
	private bool isNewScene=false;
	private windowSetting Setting;
	private int hitCount=0;
	private int hitEnemies=0;
	private bool DebugMode=false;
	private TextMesh HitResult;
	private TextMesh TimerObj;
	private Depth depth;
	private bool isMirrorMode;
	private bool LogView=false;
	private MeshRenderer LogViewMesh;
	private TextMesh LogViewText;
	// Use this for initialization
	void Awake(){
		DontDestroyOnLoad (this);
		Application.targetFrameRate = 120;
	}
	void Start () {
		newScene ();
	}
	// Update is called once per frame
	void Update () {
		if (isNewScene)
			newScene ();
		timer += Time.deltaTime;
		if (TimerObj != null) {
			float timeView=setTime-Mathf.Floor(timer);
			TimerObj.text=timeView.ToString()+"sec";
		}
		//結果保存して遷移
		if (setTime <= timer && Application.loadedLevelName=="Main") {
			changeScene("Result");
		}
		else if(10f<=timer && Application.loadedLevelName=="Result")
		{
			changeScene("Title2");
		}
		if (depth!=null) {
			hitCount=depth.getHitCount();
			hitEnemies=depth.getHitEnemies();
			depth.setMirrorMode (isMirrorMode);
		}
		LogViewMesh.enabled=LogView;
			LogViewText.text=	"HitCount:"+hitCount+"\n"+
								"HitEnemy:"+hitEnemies+"\n";
		checkInputKey ();
	}

	void newScene(){
		oparate = GameObject.Find ("Operating").GetComponent<MeshRenderer>();
		oparate.enabled=false;
		if (Application.loadedLevelName == "Title"||Application.loadedLevelName == "Title2") {
			Setting = GameObject.Find ("Window").GetComponent<windowSetting> ();
			dash = this.GetComponent<AudioSource> ();
		} else if (Application.loadedLevelName == "Main") {
			oparate.enabled = false;
			TimerObj=GameObject.Find("Timer").GetComponent<TextMesh>();
			playerScript = GameObject.Find ("Player").GetComponent<KinectPlayer> ();
			depth = GameObject.Find ("Enemy").GetComponent<Depth> ();
			depth.reset ();
		} else if (Application.loadedLevelName == "Result") {
			HitResult=GameObject.Find("HitCount").GetComponent<TextMesh>();
			string people="people!";
			if(hitEnemies<=1)
				people="person!";
			HitResult.text=	hitCount.ToString()+"Hit!"+"\n"+
							hitEnemies.ToString()+people+"\n"+
							"Thank you for Playing!!";
		}
		GameObject LogViewObj = GameObject.Find ("Log");
		LogViewMesh = LogViewObj.GetComponent<MeshRenderer> ();
		LogViewText = LogViewObj.GetComponent<TextMesh> ();
		gameStart = false;
		timer = 0f;
		isNewScene = false;
	}


	void checkInputKey()
	{
		//ゲーム終了
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
		//プレイヤーのトラッキングをリセット
		else if (Input.GetKeyDown (KeyCode.Space)) {
			if (Application.loadedLevelName == "Main")
				playerScript.resetPlayer ();
		} else if (Input.GetKeyDown (KeyCode.R)) {
			changeScene("Title2");
		}
		//ヘルプを表示
		else if (Input.GetKeyDown (KeyCode.H)) {
			if(oparate.isVisible)
				oparate.enabled=false;
			else
				oparate.enabled=true;
		}
		//デバッグモードで画面遷移確認
		else if (DebugMode && Input.GetKeyDown (KeyCode.N)) {
			Debug.Log(Application.loadedLevelName);
			switch(Application.loadedLevelName)
			{
			case "Title":
			case "Title2":
				changeScene("Main");
				break;
			case "Main":
				changeScene("Result");
				break;
			case "Result":
				changeScene("Title2");
				break;
			}
		}
		//設定ウィンドウ表示
		else if (Input.GetKeyDown (KeyCode.LeftShift)||Input.GetKeyDown(KeyCode.RightShift)) {
			Setting.enableWindow();
		}
		//ゲーム開始モード
		else if (Application.loadedLevelName == "Title"||Application.loadedLevelName == "Title2") {
			if (Input.GetKeyDown (KeyCode.S)) {
				dash.Play ();
				gameStart = true;
			}
			if (!dash.isPlaying && gameStart) {
				changeScene ("Main");
			}
		}
	}




	/// <summary>
	/// シーン切り替え時処理をまとめておく。
	/// </summary>
	void changeScene(string moveScene){
		isNewScene = true;
		Application.LoadLevel (moveScene);
	}
	public float getTimer()
	{
		return setTime;
	}
	public void setTimer(float newTimer){
		setTime = newTimer;
	}
	public void getDebugMode(bool newDebugMode){
		DebugMode = newDebugMode;
	}
	public void setMirrorMode(bool MirrorMode)
	{
		isMirrorMode = MirrorMode;
	}
	public void isViewLog(bool isLogView){
		LogView = isLogView;
	}
}
