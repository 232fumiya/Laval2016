using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect=Windows.Kinect;
public class KinectPlayer : MonoBehaviour {

	BodySourceManager data;
	private Kinect.Body[] player=null;
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
	private float catchTimer=0f;
	/// <summary>
	/// -1=notCatch 0=Catch 1=ShootWait 2=Shoot
	/// </summary>
	private int ShootState = -1;
	private bool isShootMode=false;
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
		if (player == null) {
			Time.timeScale=0f;
			return;
		}
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
						Time.timeScale=1.0f;
					}
				}
			}
			if (PlayerPos == -1000f) {
				playerIsTracking=false;
			}
			return;
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
			StartCoroutine("ShootMode");
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
		rightHandObj.transform.rotation = Quaternion.Euler (new Vector3(-90,90,0));
		leftHandObj.transform.rotation = Quaternion.Euler (new Vector3(90,-90,0));
		//this.transform.position = new Vector3 (0,-10,0);
		float dist = Vector3.Distance (leftHandObj.transform.position,rightHandObj.transform.position);
		float sizeChangeLine = newSnow.transform.localScale.x + 3;
		if (newSnow.transform.localScale.z < 0.5f) {
			sizeChangeLine += 2f;
			float z = (leftHandObj.transform.localPosition.z+rightHandObj.transform.localPosition.z)/2;
			float x = (leftHandObj.transform.localPosition.x + rightHandObj.transform.localPosition.x) / 2;
			snowScript.handCenterPos(new Vector3 (x,0.1f,z));
		}
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
		if (catchTimer < 1f) {
			catchTimer += Time.deltaTime;
			newSnow.transform.position=Vector3.Slerp(newSnow.transform.position,makeSnowPos(),catchTimer);
			return;
		}
		if (isRightHandCatch) {
			newSnow.transform.position = makeSnowPos();
			checkShoot(elbowRight,rightHandObj);
		} else {
			newSnow.transform.position = makeSnowPos();
			checkShoot(elbowLeft,leftHandObj);
		}
	}

	private void checkShoot(Vector3 elbow,GameObject hand){
		Vector3 hand_elbow=hand.transform.localPosition-elbow;
		hand_elbow=hand_elbow.normalized;
		float dot=Vector3.Dot(Vector3.forward,hand_elbow);
		if (hand_elbow.y > 0) {
			if (dot < 0.6f && ShootState == 0) {
				ShootState = 1;
				if(isRightHandCatch)
					rightHandObj.transform.rotation = Quaternion.Euler (new Vector3(180,90,90));
				else
					leftHandObj.transform.rotation = Quaternion.Euler (new Vector3(0,-90,90));
			} else if (0.7f < dot && ShootState == 1) {
				ShootState = 2;
				State = 2;
				catchTimer=0f;
			}
		}
	}
	private IEnumerator ShootMode(){
		if (isShootMode)
			yield break;
		else
			isShootMode = true;
		Vector3 slowVec = Vector3.zero;
		if (isRightHandCatch) {
			slowVec.x = (rightHandObj.transform.position.x - elbowRight.x);
			slowVec.y = rightHandObj.transform.position.y;
			rightHandObj.transform.rotation = Quaternion.Euler (new Vector3(180,90,0));
		} else {
			slowVec.x = (leftHandObj.transform.position.x - elbowLeft.x);
			slowVec.y = leftHandObj.transform.position.y;
			leftHandObj.transform.rotation = Quaternion.Euler (new Vector3(0,-90,0));
		}
		slowVec = new Vector3 (slowVec.x*5,slowVec.y+10,50);
		snowRigid.isKinematic = false;
		snowRigid.AddForce (slowVec, ForceMode.Impulse);
		snowScript.isShooting (true);
		yield return new WaitForSeconds (0.5f);
		ShootState = -1;
		State = -1;
		newSnow = null;
		isShootMode = false;
	}
	public void getTouch(){
		if (newSnow.transform.localScale.x < 0.5f)
			return;
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

	private Vector3 makeSnowPos()
	{
		Vector3 snowpos = Vector3.zero;
		if (isRightHandCatch){
			snowpos = rightHandObj.transform.position + new Vector3 (-1, 0, 1);
			if(ShootState==1)
				snowpos = rightHandObj.transform.position + new Vector3 (0,1,1);
		}
		else{
			snowpos = leftHandObj.transform.position + new Vector3 (1, 0, 1);
			if(ShootState==1)
				snowpos = leftHandObj.transform.position + new Vector3(0,1,1);
		}
		return snowpos;
		
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
		if (newSnow != null) {
			Destroy(newSnow.gameObject);
			newSnow = null;
		}
	}
}
