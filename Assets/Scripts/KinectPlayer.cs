using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect=Windows.Kinect;
public class KinectPlayer : MonoBehaviour {

	BodySourceManager data;
	private Kinect.Body[] player=null;
	private bool isHandTrack=false;
	private GameObject rightHandObj,leftHandObj;
	private int playerNum=0;
	private bool playerIsTracking=false;

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
		if (!player [playerNum].IsTracked||!playerIsTracking) {
			float PlayerPos = -1000f;
			for (int i=0; i<player.Length; i++) {	
				Kinect.Body body = player [i];
				if (body == null)
					return;
				if (body.IsTracked) {
					Kinect.Joint Head = body.Joints [Kinect.JointType.Head];
					Vector3 HeadPos = GetVector3FromJoint (Head);
					if (PlayerPos < HeadPos.z) {
						PlayerPos = HeadPos.z;
						playerNum = i;
						playerIsTracking=true;
					}
				}
			}
			if (PlayerPos == -1000f) {
				playerIsTracking=false;
			}
		}
		else if (player[playerNum].IsTracked) {
			activeHandObj(true);
			Kinect.Joint LeftHand = player [playerNum].Joints [Kinect.JointType.HandLeft];
			Kinect.Joint RightHand = player [playerNum].Joints [Kinect.JointType.HandRight];			
			Kinect.Joint RightElbow=player[playerNum].Joints[Kinect.JointType.ElbowRight];
			Kinect.Joint LeftElbow=player[playerNum].Joints[Kinect.JointType.ElbowLeft];
			leftHandObj.transform.localPosition = GetVector3FromJoint (LeftHand);
			rightHandObj.transform.localPosition= GetVector3FromJoint (RightHand);
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
		//this.transform.position = new Vector3 (0,-10,0);
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
		//this.transform.position = new Vector3 (0,-5,0);
		if (isRightHandCatch) {
			newSnow.transform.position = rightHandObj.transform.position;
			checkShoot(elbowRight,rightHandObj);
		} else {
			
			newSnow.transform.position = leftHandObj.transform.position;
			checkShoot(elbowLeft,leftHandObj);
		}
	}

	private void checkShoot(Vector3 elbow,GameObject hand){
		Vector3 hand_elbow=hand.transform.localPosition-elbow;
		hand_elbow=hand_elbow.normalized;
		float dot=Vector3.Dot(Vector3.forward,hand_elbow);
		if (hand_elbow.y > 0) {
			if (dot < 0.4f && ShootState == 0 && ShootStateCount < 10) {
				ShootState = 1;
			} else if (0.6f < dot && ShootState == 1) {
				ShootState = 2;
				State = 2;
			}
		}
	}

	private void ShootMode(){
		Vector3 slowVec = Vector3.zero;
		slowVec.x = (rightHandObj.transform.position.x - elbowRight.x);
		slowVec = new Vector3 (slowVec.x*5,10,50);
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
		return new Vector3(joint.Position.X * 10, joint.Position.Y * 10 +15, -joint.Position.Z * 10);
	}
	public int getPlayerNum(){
		return playerNum;
	}

	/// <summary>
	/// -2=notTrack  -1=notTouch  0=touch  1=catch  2=shoot
	/// </summary>
	public int getState(){
		return State;
	}
	/// <summary>
	/// isRightHand
	/// </summary>
	public bool getWitchHands()
	{
		return isRightHandCatch;
	}
	public void resetPlayer()
	{
		playerIsTracking = false;
	}
}
