using UnityEngine;
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

	//ScrapeHand
	private float beforeHandDist;
	private int sizeChangeCounter;


	//CatchHandMode
	private bool isRightHandCatch=false;


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
			activeHandObj(true);
			Kinect.Joint LeftHand = player [playerNum].Joints [Kinect.JointType.HandLeft];
			Kinect.Joint RightHand = player [playerNum].Joints [Kinect.JointType.HandRight];
			leftHandObj.transform.position = GetVector3FromJoint (LeftHand);
			rightHandObj.transform.position = GetVector3FromJoint (RightHand);
		} else {
			activeHandObj(false);
			return;
		}
		if (newSnow == null)
			CreateSnow ();
		switch (State) {
		case 0:
			ScrapeHandMode();
			break;
		case 1:
			CatchMode();
			break;
		case 2:
			ShootWaitMode();
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

	private void activeHandObj (bool isTracking){
		leftHandObj.SetActive (isTracking);
		rightHandObj.SetActive (isTracking);
	}

	private void ScrapeHandMode(){
		if (newSnow == null) {
			State = -1;
			return;
		}
		float dist = Vector3.Distance (leftHandObj.transform.position,rightHandObj.transform.position);
		float sizeChangeLine = newSnow.transform.localScale.x + 5;
		if (dist <= beforeHandDist && dist <= sizeChangeLine) {
			if (sizeChangeCounter < 10) {
				float addSize = 0.06f;
				Vector3 addSizeVec = new Vector3 (addSize, addSize, addSize);
				Vector3 snowSize = newSnow.transform.localScale;
				addSizeVec += snowSize;
				snowScript.changeSnowSize (addSizeVec);
				sizeChangeCounter++;
			}
		} else if (sizeChangeLine / 2 < dist && dist<beforeHandDist) {
			sizeChangeCounter=0;
			beforeHandDist = dist;
		}
		beforeHandDist = dist;
	}

	private void CatchMode(){
		if (isRightHandCatch)
			newSnow.transform.position = rightHandObj.transform.position;
		else
			newSnow.transform.position = leftHandObj.transform.position;
	}

	private void ShootWaitMode(){

	}
	public void getTouch(){
		Debug.Log ("aaa");
		State = 1;
		float rightDist = Vector3.Distance (rightHandObj.transform.position,newSnow.transform.position);
		float leftDist = Vector3.Distance (leftHandObj.transform.position, newSnow.transform.position);
		if (rightDist < leftDist)
			isRightHandCatch = true;
		else
			isRightHandCatch = false;
	}

	private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
	{
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10 +5, -joint.Position.Z * 10);
	}
	public int getPlayerNum(){
		return playerNum;
	}
}
