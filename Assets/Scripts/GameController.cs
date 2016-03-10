using UnityEngine;
using System.Collections;
/// <summary>
/// Game controller
/// </summary>
public class GameController : MonoBehaviour {
	private float timer;
	private float setTime=60f;
	private AudioSource dash;
	private PhidgetsController phidgetController;
	private bool gameStart=false;
	private bool isChangeScene=false;
		

	// Use this for initialization
	void Start () {
		phidgetController = GameObject.Find ("PhidgetObj").GetComponent<PhidgetsController> ();
		Application.targetFrameRate = 120;
		if(Application.loadedLevelName=="Title")
			dash=this.GetComponent<AudioSource>();
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
		if (Input.GetKeyDown (KeyCode.Q)) {
			Application.Quit ();
		}
		if (Application.loadedLevelName == "Title") {
			if (Input.GetKeyDown (KeyCode.S)) {
				dash.Play ();
				gameStart = true;
			}
			if (!dash.isPlaying && gameStart) {
				StartCoroutine(changeScene("Main"));
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
