using UnityEngine;
using System.Collections;
/// <summary>
/// Game controller
/// </summary>
public class GameController : MonoBehaviour {
	private float timer;
	private float setTime=60f;
	private AudioSource dash;
	private bool gameStart=false;
	void Awake()
	{
	
		Application.targetFrameRate = 120;

	}
	// Use this for initialization
	void Start () {
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
			//Result画面作るならそっちに変更する
			Application.LoadLevel ("Title");
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel ("Title");
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
				Application.LoadLevel ("Main");
			}
		}
	}
}
