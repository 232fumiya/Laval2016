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
	private Vector3 elbowRight;
	private Vector3 elbowLeft;

	//CatchHandMode
	private bool isRightHandCatch=false;

	/// <summary>
	/// -1=notCatch 0=Catch 1=ShootWait 2=Shoot
	/// </summary>
	private int ShootState = -1;
	private int ShootStateCount=0;

	/// <summary>
	/// -2=notTracking -1=notTouch  0=TouchSnow 1=CatchSnow 2=ShootSnow
	/// </summary>
	private int State = -2;
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
			Kinect.Joint RightElbow=player[playerNum].Joints[Kinect.JointType.ElbowRight];
			Kinect.Joint LeftElbow=player[playerNum].Joints[Kinect.JointType.ElbowLeft];
			leftHandObj.transform.position = GetVector3FromJoint (LeftHand);
			rightHandObj.transform.position = GetVector3FromJoint (RightHand);
			elbowRight=GetVector3FromJoint(RightElbow);
			elbowLeft=GetVector3FromJoint(LeftElbow);
		} else {
			activeHandObj(false);
			return;
		}
		if (newSnow == null)
			CreateSnow ();
		switch (State) {
		case 0:
		case -1:
			ScrapeHandMode();
			break;
		case 1:
			CatchMode();
			break;
		case 2:
			ShootMode();
			break;
		default:
			break;
		}
	}
	private void CreateSnow()
	{
		State = -1;
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
			State = -2;
			return;
		}
		float dist = Vector3.Distance (leftHandObj.transform.position,rightHandObj.transform.position);
		float sizeChangeLine = newSnow.transform.localScale.x + 3;
		if (newSnow.transform.localScale.z < 0.5f)
			sizeChangeLine += 2f;
		if (dist <= beforeHandDist && dist <= sizeChangeLine) {
			if (sizeChangeCounter < 10) {
				float addSize = 0.06f;
				Vector3 addSizeVec = new Vector3 (addSize, addSize, addSize);
				Vector3 snowSize = newSnow.transform.localScale;
				addSizeVec += snowSize;
				snowScript.changeSnowSize (addSizeVec);
				sizeChangeCounter++;
				State=0;
			}
			else{
				State=-1;
			}
		} else if (sizeChangeLine / 2 < dist && dist<beforeHandDist) {
			sizeChangeCounter=0;
			beforeHandDist = dist;
			State=-1;
		}
		beforeHandDist = dist;
	}

	private void CatchMode(){
		if(ShootState==-1)
			ShootState = 0;
		if (isRightHandCatch) {
			newSnow.transform.position = rightHandObj.transform.position;
			Vector3 RightHand_Elbow=rightHandObj.transform.position-elbowRight;
			RightHand_Elbow=RightHand_Elbow.normalized;
			float RightDot=Vector3.Dot(Vector3.forward,RightHand_Elbow);
			checkShoot(RightDot);
		} else {
			newSnow.transform.position = leftHandObj.transform.position;
			Vector3 LeftHand_Elbow=leftHandObj.transform.position-elbowLeft;
			LeftHand_Elbow=LeftHand_Elbow.normalized;
			float LeftDot=Vector3.Dot(Vector3.forward,LeftHand_Elbow);
			checkShoot(LeftDot);
		}
	}

	private void checkShoot(float dot){
		Debug.Log (dot);
		if (dot <= 0.3f && ShootState == 0 && ShootStateCount < 10) {
			ShootState = 1;
		} else if (0.6f<dot && ShootState == 1) {
			ShootState = 2;
			State = 2;
		}
	}

	private void ShootMode(){
		Vector3 slowVec = Vector3.zero;
		slowVec.x = (rightHandObj.transform.position.x - elbowRight.x);
		slowVec = new Vector3 (slowVec.x*5,10,20);
		snowRigid.isKinematic = false;
		snowRigid.AddForce (slowVec, ForceMode.Impulse);
		snowScript.isShooting (true);
		ShootState = -1;
		State = -1;
		newSnow = null;
	}
	public void getTouch(){
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
	public int getState(){
		return State;
	}
	public bool getWitchHands()
	{
		return isRightHandCatch;
	}
}
