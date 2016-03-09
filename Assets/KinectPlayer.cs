using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect=Windows.Kinect;
public class KinectPlayer : MonoBehaviour {

	BodySourceManager data;
	private Kinect.Body[] player=null;
	private bool isHandTrack=false;
	// Use this for initialization
	void Start () {
		data = GameObject.Find ("BodySourceManager").GetComponent<BodySourceManager> ();
	}
	// Update is called once per frame


	void Update(){
		player=data.GetData();
		if (player == null)
			return;
		foreach(Kinect.Body body in player)
		{
			if(body==null)
				return;
			if(body.IsTracked)
			{
				Kinect.Joint LeftHand=body.Joints[Kinect.JointType.HandLeft];
				Kinect.Joint RightHand=body.Joints[Kinect.JointType.HandRight];
				Debug.Log(GetVector3FromJoint(LeftHand));
				Debug.Log(GetVector3FromJoint(RightHand));

			}
		}
	}
	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
	}
}