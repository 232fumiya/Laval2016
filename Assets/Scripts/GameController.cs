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
	private bool isChangeScene=false;
	private GameObject oparate;
	// Use this for initialization
	void Start () {
		oparate = GameObject.Find ("Operating");
		phidgetController = GameObject.Find ("PhidgetObj").GetComponent<PhidgetsController> ();
		Application.targetFrameRate = 120;
		if (Application.loadedLevelName == "Title") 
		{
			oparate.SetActive(true);
			dash = this.GetComponent<AudioSource> ();
		}else if (Application.loadedLevelName == "Main") {
			oparate.SetActive(false);
			playerScript = GameObject.Find ("Player").GetComponent<KinectPlayer> ();
		}
		Cursor.visible = false;
		gameStart = false;
		timer = 0f;
	}
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (setTime <= timer) {
			StartCoroutine(changeScene("Title"));
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			StartCoroutine(changeScene(Application.loadedLevelName));
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			playerScript.resetPlayer();
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			if(oparate.activeSelf)
				oparate.SetActive(false);
			else
				oparate.SetActive(true);
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
	/// <summary>
	/// シーン切り替え時処理をまとめておく。
	/// </summary>
	IEnumerator changeScene(string moveScene){
		if (isChangeScene)
			yield break;
		else
			isChangeScene = true;
		phidgetController.PhidgetClose ();
		Application.LoadLevel (moveScene);
	}
}
