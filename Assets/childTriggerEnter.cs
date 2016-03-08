using UnityEngine;
using System.Collections;

public class childTriggerEnter : MonoBehaviour {
	GameObject parentObject;
	checkHandPoseing checkHandPoseingScript;
	// Use this for initialization
	void Start () {
		getScript ();
		this.GetComponent<Collider> ().isTrigger = true;
	}	
	// Update is called once per frame
	void Update () {
	}
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "snow") 
		{
			if(parentObject!=null)
				checkHandPoseingScript.touchSnow(true);
			else
				getScript();
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "snow") 
		{
			if(parentObject!=null)
				checkHandPoseingScript.touchSnow(false);
			else
				getScript();
		}
	}
	void getScript()
	{
		parentObject = GameObject.Find ("HandController");
		checkHandPoseingScript = parentObject.GetComponent<checkHandPoseing> ();
	}
}
