using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {
	TextMesh message;
	// Use this for initialization
	void Start () {
		message=GameObject.Find("message").GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {

	}
	public void Scrape()
	{
		message.text = "scrape together";
	}
	public void Catch()
	{
		message.text = "Shoot gesture!";
	}
	public void Shoot(){
		message.text = "Shoot!!";
	}
}
