using UnityEngine;
using System.Collections;
using Phidgets;
public class PhidgetsController : MonoBehaviour {
	/// <summary>
	/// 右手=0,1,2
	/// 左手=3,4,5
	/// </summary>
	private InterfaceKit waterController;
	private KinectPlayer playerScripts;
	private bool isRightHand=false;
	private bool isWaterControl=false;
	// Use this for initialization
	void Start () {
		if(Application.loadedLevelName=="Main")
			playerScripts=GameObject.Find("Player").GetComponent<KinectPlayer>();
		waterController = new InterfaceKit ();
		waterController.open ();
		waterController.waitForAttachment (1000);
		waterController.outputs[7]=true;
		normalMode ();
		StartCoroutine (waterControl());
	}
	
	IEnumerator waterControl(){
		if (Application.loadedLevelName != "Main")
			yield break;
		if (isWaterControl)
			yield break;
		else 
			isWaterControl = true;
		while(true)
		{
			int State= playerScripts.getState();
		//	Debug.Log(State);
			isRightHand = playerScripts.getWitchHands();
			if(!isWaterControl)
				yield break;
			switch(State)
			{
			case 0:
				touchMode();
				break;
			case 1:
				catchMode();
				break;
			case 2:
				shootMode();
				break;
			case -2:
				notTracking();
				break;
			case -1:
				normalMode();
				break;
			default:
				normalMode();
				break;
			}
			yield return new WaitForSeconds(0.1f);
		}
	}
	/// <summary>
	/// Scrape mode
	/// </summary>
	void touchMode()
	{
		waterController.outputs[0]=true;
		waterController.outputs[1]=false;
		waterController.outputs[2]=true;
		waterController.outputs[3]=true;
		waterController.outputs[4]=false;
		waterController.outputs[5]=true;
	}
	/// <summary>
	/// Catchs the mode
	/// </summary>
	void catchMode()
	{
		if (isRightHand) {
			waterController.outputs [0] = true;
			waterController.outputs [1] = false;
			waterController.outputs [2] = false;
			waterController.outputs [3] = true;
			waterController.outputs [4] = true;
			waterController.outputs [5] = true;
		} else {
			waterController.outputs [0] = true;
			waterController.outputs [1] = true;
			waterController.outputs [2] = true;
			waterController.outputs [3] = true;
			waterController.outputs [4] = false;
			waterController.outputs [5] = false;
		}
	}
	/// <summary>
	/// Shoots the mode
	/// </summary>
	void shootMode()
	{
		if (isRightHand) {
			waterController.outputs [0] = false;
			waterController.outputs [1] = true;
			waterController.outputs [2] = true;
			waterController.outputs [3] = true;
			waterController.outputs [4] = true;
			waterController.outputs [5] = true;
		} else {
			waterController.outputs [0] = true;
			waterController.outputs [1] = true;
			waterController.outputs [2] = true;
			waterController.outputs [3] = false;
			waterController.outputs [4] = true;
			waterController.outputs [5] = true;
		}
	}
	void normalMode()
	{
		waterController.outputs [0] = true;
		waterController.outputs [1] = true;
		waterController.outputs [2] = true;
		waterController.outputs [3] = true;
		waterController.outputs [4] = true;
		waterController.outputs [5] = true;
	}
	void notTracking(){
		waterController.outputs [0] = false;
		waterController.outputs [1] = false;
		waterController.outputs [2] = false;
		waterController.outputs [3] = false;
		waterController.outputs [4] = false;
		waterController.outputs [5] = false;
	}
	void OnApplicationQuit()//終了時処理
	{
		waterController.outputs[0]=false;
		waterController.outputs[1]=false;
		waterController.outputs[2]=false;
		waterController.outputs[3]=false;
		waterController.outputs[4]=false;
		waterController.outputs[5]=false;
		waterController.outputs[7]=false;
		waterController.close ();
	}
	public void PhidgetClose()
	{
		isWaterControl = false;
		waterController.close ();
	}
}