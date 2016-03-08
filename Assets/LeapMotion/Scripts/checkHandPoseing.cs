using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Leap;
public class checkHandPoseing : MonoBehaviour
{
	private bool GlowUp=false;

	private HandModel[] checkHands=new HandModel[2];
	private Vector3[] thumb = new Vector3[2]; 
	private Vector3[] index =new Vector3[2]; 
	private Vector3[] middle =new Vector3[2]; 
	private Vector3[] ring =new Vector3[2]; 
	private Vector3[] pink =new Vector3[2];
	private Vector3[] HandPalm =new Vector3[2];

	private float BeforeHandPalmsLength = 0;
	private GameObject SnowBall;
	private GameObject snowInstance;
	private Rigidbody snowRigid;
	private Vector3 palmMiddle;
	private snowballScript snowScript;
	private int snowNum=0;
	private float chargeCheck;
	private int numberOfTimes=0;
	private Vector3 ShootPos;
	private int ShootState = 0;
	private float shoottimer=0;
	private Vector3 SlowVec;
	private bool shoot=false;
	private bool checkCatch=false;
	private bool checkTouch=false;
	private bool catchRightHand=false;
	private bool zeroIsRight=false;
	private GameObject[] HandObjects =new GameObject[2];
	private int catchCounter=0;
	public GameObject Cam;
	private bool isCamRotChange=false;
	private bool shootStay=false;
	private bool isInstanceSnow=false;
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		SnowBall = Resources.Load("SnowBall") as GameObject;
	}

	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void Update ()
	{
		for (int i=0; i<2; i++){
			if (HandController.GetHands [i] != null)
			{	
				getFingerData(i);
				if(HandObjects[0]==null||HandObjects[1]==null)
				{
					GameObject HandControllerObject=GameObject.Find("HandController");
					foreach(Transform child in HandControllerObject.transform)
					{
						if(child.name=="RigidRoundHand(Clone)")
						{
							HandObjects[i]=null;
							HandObjects[i]=child.gameObject;
						}
					}
				}
			}
			else
			{
				HandObjects[0]=null;
				HandObjects[1]=null;
				checkHands [i] =null;
			}
		}
		if (checkHands [0] != null && checkHands [1] != null) {
			if(snowInstance==null)
				StartCoroutine(instanceSnow());
			else{
					checkScrapeTogether ();
					catchSnow (whichHand ());
				}
		} else if (checkCatch && (HandPalm [0] != null || HandPalm [1] != null)) {
			catchSnow (whichHand ());
		} else
			ShootState = 0;
	}

	/// <summary>
	/// Deletes the object.
	/// </summary>
	private void deleteObj()
	{
		if (snowInstance!= null&& palmMiddle!=Vector3.zero&&!checkCatch&&!shoot) 
		{
			if(10f<=Vector3.Distance(snowInstance.transform.position,palmMiddle))
			{
				Destroy(snowInstance);
				snowInstance=null;
				Debug.Log("Destroy");
			}
		}
	}

	/// <summary>
	/// Checks the slow snow.
	/// </summary>
	/// <returns>The slow snow.</returns>
	private int whichHand()
	{
		int hand = 0;
		if (snowInstance == null)
			return hand;
		if (snowInstance.transform.localScale.y > 1) 
		{
			float palm0=Vector3.Distance(HandPalm[0],snowInstance.transform.position);
			float palm1=Vector3.Distance(HandPalm[1],snowInstance.transform.position);
			if(palm0<palm1)
			{
				hand=0;
			}
			else if(palm1<palm0)
			{
				hand=1;
			}
		}
		if (HandPalm [0] != null && HandPalm [1] != null) {
			if (HandPalm [0].x < HandPalm [1].x) {
				zeroIsRight = false;
				if (hand == 0)
					catchRightHand = false;
				else
					catchRightHand = true;
			} else {
				zeroIsRight = true;
				if (hand == 0)
					catchRightHand = true;
				else
					catchRightHand = false;
			}
			//Debug.Log("0番："+HandPalm[0].x+" 1番："+HandPalm[1].x+"\n"+zeroIsRight+" "+catchRightHand);
		} else {
			if(zeroIsRight){
				if(hand==0)
					catchRightHand=true;
				else
					catchRightHand=false;
			}else{
				if(hand==0)
					catchRightHand=false;
				else
					catchRightHand=true;
			}
		}
		return hand;
	}

	/// <summary>
	/// Catchs the snow.
	/// </summary>
	/// <param name="rightOrLeft">Right or left.</param>
	void catchSnow(int rightOrLeft)
	{
		if (snowInstance == null)
			return;
		if (!checkCatch)
			return;
		Vector3 checkSnowPos=(thumb[rightOrLeft]+pink[rightOrLeft])/2;
		float getSnowSize = snowInstance.transform.localScale.x/5;
		if(Vector3.Distance(thumb[rightOrLeft],pink[rightOrLeft])<=getSnowSize*10&&!shoot)
		{
			//SnowBall.transform.LookAt(HandPalm[rightOrLeft]);
			checkSnowPos+=new Vector3(getSnowSize,0,0);
			snowInstance.transform.position=checkSnowPos;
			Cam.transform.rotation = Quaternion.Euler (0,0,0);
			shootStay=true;
		}
		Vector3 ShootDot = middle [rightOrLeft] - HandPalm [rightOrLeft];
		Vector3 checkShoot = Vector3.forward;
		float checkShootDot = Vector3.Dot (ShootDot.normalized, checkShoot);
		shoottimer+=Time.deltaTime;
		if (checkShootDot < 0.5f && ShootState == 0 && 0.5f < shoottimer) {
			shoottimer = 0;
			ShootState = 1;
		}
		else if (0.8f <= checkShootDot && ShootState == 1) {
			ShootState = 0;
			ShootSnow (rightOrLeft);
		}
	}

	/// <summary>
	/// Shoots the snow.
	/// </summary>
	/// <param name="rightOrLeft">Right or left.</param>
	void ShootSnow(int rightOrLeft)
	{
		if (!checkCatch)
			return;
		shoot = true;
		SlowVec = Vector3.zero;
		SlowVec.x = middle [rightOrLeft].x-HandPalm[rightOrLeft].x;
		if (SlowVec.z < 0)
			SlowVec.z = -SlowVec.z;
		SlowVec+= new Vector3 (SlowVec.x*20, 10,30);
		//Debug.Log(SlowVec);
		snowRigid.isKinematic = false;
		snowInstance.transform.rotation= Quaternion.Euler(0,0,0);
		snowRigid.AddForce(SlowVec,ForceMode.Impulse);
		snowScript.isShooting (true);
		checkCatch = false;
		snowInstance = null;
		shootStay = false;
	}
	/// <summary>
	/// Checks the scrape together.
	/// </summary>
	void checkScrapeTogether()
	{
		deleteObj ();
		for (int j=0; j<2; j++) 
		{
			//Debug.Log(HandDot);

			if(thumb[j].y>=pink[j].y&&
			   checkOpenHand(j)&&
			   checkDot(j))
			{
				scrapeTogether();
			}
		}

	}
	/// <summary>
	/// Checks the dot.
	/// </summary>
	/// <returns><c>true</c>, if dot was checked, <c>false</c> otherwise.</returns>
	/// <param name="rightOrleft">Right or Left.</param>
	bool checkDot(int rightOrleft)
	{
		bool handDotState = false;
		var thumb_pink=thumb[rightOrleft]-pink[rightOrleft];
		var hikaku=new Vector3(1,0,0);
		float DotLine=0.5f;
		float HandDot=Vector3.Dot(hikaku,thumb_pink.normalized);
		if (-DotLine <=HandDot&&
		    HandDot<= DotLine) 
		{
			handDotState=true;
		}
		return handDotState;
	}

	/// <summary>
	/// check Scrapes the together Hands.
	/// </summary>
	void scrapeTogether()
	{
		if (snowInstance == null)
			return;
		StartCoroutine ("camRotChange");
		float HandPalmsLength=Vector3.Distance(HandPalm[0],HandPalm[1]);
		float snowScrapeTogetherLine = snowInstance.transform.localScale.x+3;
		if(BeforeHandPalmsLength!=0)
		{
			if(snowInstance.transform.localScale.x<=0.5)
			{
				snowScrapeTogetherLine+=2;
			}
			if(HandPalmsLength<=BeforeHandPalmsLength&&HandPalmsLength<=snowScrapeTogetherLine)
			{
				chargeCheck++;
				if(chargeCheck>=5&&numberOfTimes<10)
				{
					float AddBallSize=0.06f;
					Vector3 AddSizeVec=new Vector3(AddBallSize,AddBallSize,AddBallSize);
					float BallSizeX=snowInstance.transform.localScale.x;
					float BallSizeY=snowInstance.transform.localScale.y;
					float BallSizeZ=snowInstance.transform.localScale.z;
					snowScript.changeSnowSize(new Vector3(BallSizeX,BallSizeY,BallSizeZ)+AddSizeVec);
					chargeCheck=0;
					checkTouch=true;
					numberOfTimes++;
				}
			}
			else if(snowScrapeTogetherLine/2<HandPalmsLength)
			{
				numberOfTimes=0;
				chargeCheck=0;
				checkTouch=false;
			}
		}
		else
		{
			BeforeHandPalmsLength=HandPalmsLength;
		}
	}


	/// <summary>
	/// Cams the rotation change.
	/// </summary>
	/// <returns>Cam rotation change.</returns>
	private IEnumerator camRotChange()
	{
		if (isCamRotChange)
			yield break;
		else
			isCamRotChange = true;
		Vector3 camRot = Cam.transform.rotation.eulerAngles;
		while (shootStay) {
			yield return null;
		}
		while (camRot.x<20f) {
			camRot.x+=1f;
			Cam.transform.rotation = Quaternion.Euler (camRot.x, 0, 0);
			yield return null;
		}
		isCamRotChange = false;
		yield break;

	}


	/// <summary>
	/// Checks the open hand.
	/// </summary>
	/// <returns><c>true</c>, if open hand was checked, <c>false</c> otherwise.</returns>
	/// <param name="HandNumber">Hand number.</param>
	bool checkOpenHand(int HandNumber)
	{
		bool checkHands = false;
		float checkLine = 1f;
		if(Vector3.Distance(thumb[HandNumber],HandPalm[HandNumber])>=checkLine&&
		   Vector3.Distance(index[HandNumber],HandPalm[HandNumber])>=checkLine&&
		   Vector3.Distance(middle[HandNumber],HandPalm[HandNumber])>=checkLine&&
		   Vector3.Distance(ring[HandNumber],HandPalm[HandNumber])>=checkLine&&
		   Vector3.Distance(pink[HandNumber],HandPalm[HandNumber])>=checkLine
		  )
		{
			checkHands=true;
		}
		return checkHands;
	}

	/// <summary>
	/// Instances the snow.
	/// </summary>
	IEnumerator instanceSnow()
	{
		if (isInstanceSnow)
			yield break;
		isInstanceSnow = true;
		if (shoot) {
			yield return new WaitForSeconds (0.5f);
			shoot=false;
		}if (snowInstance != null)
			yield break;
		if (snowScript != null)
			snowScript = null;
		snowInstance=Instantiate (SnowBall,new Vector3(0,100,0),this.transform.rotation) as GameObject;
		snowNum++;
		snowRigid = snowInstance.GetComponent<Rigidbody> ();
		snowRigid.isKinematic = true;
		snowScript = snowInstance.GetComponent<snowballScript> ();
		snowScript.changeSnowSize (new Vector3 (0, 0, 0));
		snowScript.sendSnowNum (snowNum);
		//snow Pos
		palmMiddle.x = (HandPalm [0].x + HandPalm [1].x) / 2;
		palmMiddle.y = 0.1f;
		palmMiddle.z = (HandPalm [0].z + HandPalm [1].z) / 2;
		snowScript.handCenterPos (palmMiddle);
		isInstanceSnow = false;
	}



	/// <summary>
	/// Touchs the snow.
	/// </summary>
	/// <param name="touch">If set to <c>true</c> touch.</param>
	public void touchSnow(bool touch)
	{
		checkCatch=touch;
	}
	/// <summary>
	/// Gets the state
	/// </summary>
	/// <returns>touch=0,catch=1,shoot=2,other=-1</returns>
	public int getState ()
	{

		int State = -1;
		if (shoot) {
			State = 2;
		} else if (checkCatch) {
			State = 1;
		} else if (checkTouch) {
			State=0;
		}
		return State;

	}
	public bool isRightCatching()
	{
		return catchRightHand;
	}

	/// <summary>
	/// Gets the finger data.
	/// </summary>
	/// <param name="rightOrleft">Right or Left.</param>
	void getFingerData(int rightOrleft)
	{
		checkHands [rightOrleft] = HandController.GetHands [rightOrleft];
		thumb[rightOrleft]=checkHands[rightOrleft].fingers[0].GetTipPosition();
		index[rightOrleft]=checkHands[rightOrleft].fingers [1].GetTipPosition();
		middle[rightOrleft]=checkHands[rightOrleft].fingers [2].GetTipPosition();
		ring[rightOrleft]=checkHands[rightOrleft].fingers [3].GetTipPosition();
		pink[rightOrleft]=checkHands[rightOrleft].fingers [4].GetTipPosition();
		HandPalm[rightOrleft] = checkHands[rightOrleft].GetPalmPosition();
	}
}