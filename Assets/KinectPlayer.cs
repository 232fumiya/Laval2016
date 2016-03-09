using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect=Windows.Kinect;
public class KinectPlayer : MonoBehaviour {

	BodySourceManager data;
	private Kinect.Body[] player=null;
	private bool isHandTrack=false;
	private GameObject right,left;
	// Use this for initialization
	void Start () {
		data = GameObject.Find ("BodySourceManager").GetComponent<BodySourceManager> ();
		right = GameObject.Find ("Right");
		left = GameObject.Find ("Left");
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
				left.transform.position=GetVector3FromJoint(LeftHand);
				right.transform.position=GetVector3FromJoint(RightHand);
				Debug.Log("Left:"+GetVector3FromJoint(LeftHand)+"\n"+"Right:"+GetVector3FromJoint(RightHand));
			}
		}
	}
	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10 +5, -joint.Position.Z * 10);
	}
}