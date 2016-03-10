using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect=Windows.Kinect;
public class KinectPlayer : MonoBehaviour {

	BodySourceManager data;
	private Kinect.Body[] player=null;
	private bool isHandTrack=false;
	private GameObject right,left;
	private int playerNum=-1;
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
		for (int i=0; i<player.Length; i++) {	
			Kinect.Body body = player [i];
			if (body == null)
				return;
			float PlayerPos = -1000f;
			if (body.IsTracked) {
				Kinect.Joint Head = body.Joints [Kinect.JointType.Head];
				Vector3 HeadPos = GetVector3FromJoint (Head);
				if (PlayerPos < HeadPos.z)
				{
					playerNum=i;
					PlayerPos = HeadPos.z;
				}
				else if(PlayerPos==-1000f)
				{
					playerNum=-1;
				}
			}
		}
		if (playerNum != -1) {
			Kinect.Joint LeftHand = player[playerNum].Joints [Kinect.JointType.HandLeft];
			Kinect.Joint RightHand = player[playerNum].Joints [Kinect.JointType.HandRight];
			left.transform.position = GetVector3FromJoint (LeftHand);
			right.transform.position = GetVector3FromJoint (RightHand);
		}
	}
	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10 +5, -joint.Position.Z * 10);
	}
	public void getPlayerNum(){
		return 
	}
	}
}
