using UnityEngine;
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
	private MeshRenderer oparate;
	private bool isNewScene=false;
	private windowSetting Setting;
	private int hitCount=0;
	private bool DebugMode=false;
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
		if (setTime <= timer && Application.loadedLevelName=="Main") {
			changeScene("Result");
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			if(Application.loadedLevelName=="Main")
				playerScript.resetPlayer();
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			if(oparate.isVisible)
				oparate.enabled=false;
			else
				oparate.enabled=true;
		}
		if (DebugMode && Input.GetKeyDown (KeyCode.N)) {
			Debug.Log(Application.loadedLevelName);
		switch(Application.loadedLevelName)
			{
			case "Title":
				changeScene("Main");
				break;
			case "Main":
				changeScene("Result");
				break;
			case "Result":
				changeScene("Title");
				break;
			}
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
				changeScene ("Main");
			}
		}
	}
	void newScene(){
		oparate = GameObject.Find ("Operating").GetComponent<MeshRenderer>();
		oparate.enabled=false;
		if(Application.loadedLevelName!="Result")
			phidgetController = GameObject.Find ("PhidgetObj").GetComponent<PhidgetsController> ();
		if (Application.loadedLevelName == "Title") 
		{
			Setting=GameObject.Find("Window").GetComponent<windowSetting>();
			dash = this.GetComponent<AudioSource> ();
		}else if (Application.loadedLevelName == "Main") {
			oparate.enabled=false;
			playerScript = GameObject.Find ("Player").GetComponent<KinectPlayer> ();
		}
		gameStart = false;
		timer = 0f;
		isNewScene = false;
	}
	/// <summary>
	/// シーン切り替え時処理をまとめておく。
	/// </summary>
	void changeScene(string moveScene){
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
	public void getDebugMode(bool newDebugMode){
		DebugMode = newDebugMode;
	}
}
