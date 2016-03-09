using UnityEngine;
using System.Collections;
using Windows.Kinect;
public class KinectPlayer : MonoBehaviour {

	BodySourceManager body;
	private Body[] player=null;
	// Use this for initialization
	void Start () {
		body = GameObject.Find ("BodySourceManager").GetComponent<BodySourceManager> ();
	}
	// Update is called once per frame
	void Update () {
		player=body.GetData ();

	}
}