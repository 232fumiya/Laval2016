﻿using UnityEngine;
using System.Collections;
/// <summary>
/// Game controller
/// </summary>
public class GameController : MonoBehaviour {
	private float timer;
	private float setTime=10000f;
	private AudioSource dash;
	private PhidgetsController phidgetController;
	private KinectPlayer playerScript;
	private bool gameStart=false;
	private bool isChangeScene=false;
	private GameObject oparate;
	private bool isNewScene=false;
	private windowSetting Setting;
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
		if (setTime <= timer) {
			StartCoroutine(changeScene("Title"));
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			if(Application.loadedLevelName=="Main")
				playerScript.resetPlayer();
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			if(oparate.activeSelf)
				oparate.SetActive(false);
			else
				oparate.SetActive(true);
		}
		if (Input.GetKeyDown (KeyCode.LeftShift)||Input.GetKeyDown(KeyCode.RightShift)) {
			Setting.enableWindow();
		}
		if (Application.loadedLevelName == "Title") {
			if (Input.GetKeyDown (KeyCode.S)) {
				dash.Play ();
				gameStart = true;
			}
			if (!dash.isPlaying && gameStart) {
				StartCoroutine (changeScene ("Main"));
			}
		}
	}
	void newScene(){
		oparate = GameObject.Find ("Operating");
		phidgetController = GameObject.Find ("PhidgetObj").GetComponent<PhidgetsController> ();
		if (Application.loadedLevelName == "Title") 
		{
			Setting=GameObject.Find("Window").GetComponent<windowSetting>();
			oparate.SetActive(false);
			dash = this.GetComponent<AudioSource> ();
			Cursor.visible = true;
		}else if (Application.loadedLevelName == "Main") {
			oparate.SetActive(false);
			playerScript = GameObject.Find ("Player").GetComponent<KinectPlayer> ();
			Cursor.visible = false;
		}
		gameStart = false;
		timer = 0f;
		isNewScene = false;
	}
	/// <summary>
	/// シーン切り替え時処理をまとめておく。
	/// </summary>
	IEnumerator changeScene(string moveScene){
		if (isChangeScene)
			yield break;
		else
			isChangeScene = true;
		phidgetController.PhidgetClose ();
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
}
