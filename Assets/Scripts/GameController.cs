using UnityEngine;
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
	private float Dot;
	private bool skipTutorial=false;
	private bool ja=false;
	private bool fr=false;
	private bool en=false;
	// Use this for initialization
	void Awake(){
		DontDestroyOnLoad (this);
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
		if (setTime <= timer && Application.loadedLevelName == "Main") {
			changeScene ("Result");
		} else if (10f <= timer && Application.loadedLevelName == "Result") {
			changeScene ("Title2");
		} else if (5f <= timer && Application.loadedLevelName == "Tutorial") {
			changeScene("Main");
		}
		if (depth!=null) {
			hitCount=depth.getHitCount();
			hitEnemies=depth.getHitEnemies();
			depth.setMirrorMode (isMirrorMode);
		}
		LogViewMesh.enabled=LogView;
		if (playerScript != null) {
			Dot = playerScript.getDot ();
			Dot = Mathf.Floor(Dot*10);
			Dot=Dot/10;
		}
		LogViewText.text=	"HitCount:"+hitCount+"\n"+
							"HitEnemy:"+hitEnemies+"\n"+
							"Hand-Elbow dot:"+Dot;
		checkInputKey ();
	}

	void newScene(){
		Application.targetFrameRate = 60;
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
		} else if (Application.loadedLevelName == "Result" && HitResult==null) {
			HitResult=GameObject.Find("HitCount").GetComponent<TextMesh>();
			if(en){
				string people="people!";
				if(hitEnemies<=1)
					people="person!";
				HitResult.text=	hitCount.ToString()+"Hit!"+"\n"+
								hitEnemies.ToString()+people+"\n"+
								"Thank you for Playing!!";
			}else if(fr){
				string people="people!";
				if(hitEnemies<=1)
					people="person!";
				HitResult.text=	hitCount.ToString()+"Hit!"+"\n"+
								hitEnemies.ToString()+people+"\n"+
								"Thank you for Playing!!";
			}else if(ja){
				HitResult.text=	hitCount.ToString()+"回当たりました！"+"\n"+
								hitEnemies.ToString()+"人に当たりました！"+"\n"+
								"また遊んでね！！";
			}
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
		if (Input.GetKeyDown (KeyCode.Q)) {
			Setting.setSafety();
		}
		//ゲーム終了
		else if (Input.GetKeyDown (KeyCode.Escape)) {
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
				changeScene("Tutorial");
				break;
			case "Tutorial":
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
				if(!skipTutorial)
					changeScene ("Tutorial");
				else
					changeScene("Main");
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
	public void setSkipTutorialMode(bool skip)
	{
		skipTutorial = skip;
	}
	public void setLng(string lang)
	{
		switch (lang) {
		case "Japanese":
			fr = false;
			en = false;
			ja=true;
			break;
		case "French":
			ja=false;
			en=false;
			fr=true;
			break;
		case "English":
			ja = false;
			fr = false;
			en=true;
			break;
		}
	}
	public void setAddSize(float AddSize)
	{
		if(playerScript!=null)
			playerScript.setAdd (AddSize);
	}
}
