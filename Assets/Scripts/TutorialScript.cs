using UnityEngine;
using System.Collections;

public class TutorialScript : MonoBehaviour {
	TextMesh message;
	private bool JaVer=false;
	private bool EngVer=false;
	private bool FrVer=true;
	// Use this for initialization
	void Start () {
		message=GameObject.Find("message").GetComponent<TextMesh> ();
	}
	
	// Update is called once per frame
	void Update () {

	}
	public void Scrape()
	{
		if (JaVer)
			message.text = "雪をかき集めてください";
		else if (EngVer)
			message.text = "scrape together";
		else if (FrVer)
			message.text = "Please ratisser la neige";
	}
	public void Catch()
	{
		if (JaVer)
			message.text = "この動きで雪を投げます";
		else if (EngVer)
			message.text = "Shoot gesture";
		else if (FrVer)
			message.text = "S'il vous plaît jeter la neigee";
	}
	public void Shoot(){
		if (JaVer)
			message.text = "雪を投げました！";
		else if (EngVer)
			message.text = "Shoot";
		else if (FrVer)
			message.text = "Vous pouvez jeter la neige";
	}
	public void stLang(bool ja , bool en,bool fr)
	{
		JaVer = ja;
		EngVer = en;
		FrVer = fr;
	}
}
