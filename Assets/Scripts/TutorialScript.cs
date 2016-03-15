using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {
	Animator anim;
	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
		anim.StopPlayback ();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
