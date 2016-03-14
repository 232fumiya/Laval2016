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
	private TextMesh HitResult;
	private Depth depth;
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
		//結果保存して遷移
		if (setTime <= timer && Application.loadedLevelName=="Main") {
			hitCount=depth.getHitCount();
			changeScene("Result");
		}
		else if(10f<=timer && Application.loadedLevelName=="Result")
		{
			changeScene("Title2");
		}

		checkInputKey ();
	}

	void newScene(){
		oparate = GameObject.Find ("Operating").GetComponent<MeshRenderer>();
		oparate.enabled=false;
		if(Application.loadedLevelName!="Result")
			phidgetController = GameObject.Find ("PhidgetObj").GetComponent<PhidgetsController> ();
		if (Application.loadedLevelName == "Title"||Application.loadedLevelName == "Title2") {
			Setting = GameObject.Find ("Window").GetComponent<windowSetting> ();
			dash = this.GetComponent<AudioSource> ();
		} else if (Application.loadedLevelName == "Main") {
			oparate.enabled = false;
			playerScript = GameObject.Find ("Player").GetComponent<KinectPlayer> ();
			depth = GameObject.Find ("Enemy").GetComponent<Depth> ();
			depth.reset ();
		} else if (Application.loadedLevelName == "Result") {
			HitResult=GameObject.Find("HitCount").GetComponent<TextMesh>();
			HitResult.text=hitCount.ToString()+"Hit!"+"\n"+"Thank you for Playing!!";
		}
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
			if(Application.loadedLevelName=="Main")
				playerScript.resetPlayer();
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
