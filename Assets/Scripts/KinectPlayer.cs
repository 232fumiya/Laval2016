﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect=Windows.Kinect;
public class KinectPlayer : MonoBehaviour {

	BodySourceManager data;
	private Kinect.Body[] player=null;
	private bool isHandTrack=false;
	private GameObject rightHandObj,leftHandObj;
	private int playerNum=-1;

	private GameObject snowObj;
	private GameObject newSnow;
	private Rigidbody snowRigid;
	snowballScript snowScript;

	/// <summary>
	/// -1=notTracking 0=CreateSnow 1=CatchSnow 2=ShootSnow
	/// </summary>
	private int State = -1;
	// Use this for initialization
	void Start () {
		data = GameObject.Find ("BodySourceManager").GetComponent<BodySourceManager> ();
		rightHandObj = GameObject.Find ("Right");
		leftHandObj = GameObject.Find ("Left");
		snowObj = Resources.Load ("SnowBall")as GameObject;
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
			leftHandObj.transform.position = GetVector3FromJoint (LeftHand);
			rightHandObj.transform.position = GetVector3FromJoint (RightHand);
		}
		if (newSnow == null)
			CreateSnow ();
		switch (State) {
		case 0:
			break;
		case 1:
			break;
		case 2:
			break;
		default:
			break;
		}
	}
	private void CreateSnow()
	{
		State = 0;
		newSnow=Instantiate(snowObj,new Vector3(0,0,0),Quaternion.identity)as GameObject;
		snowScript = newSnow.GetComponent<snowballScript> ();
		snowRigid = newSnow.GetComponent<Rigidbody> ();
		snowRigid.isKinematic = true;
		snowScript.changeSnowSize(Vector3.zero);
		float z = (leftHandObj.transform.position.z + rightHandObj.transform.position.z) / 2;
		snowScript.handCenterPos(new Vector3 (0,0.1f,z));
	}
	private void ScrapeHand()
	{

	}
	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10 +5, -joint.Position.Z * 10);
	}
	public int getPlayerNum(){
		return playerNum;
	}
}
