
using UnityEngine;
using System.Collections;
using Phidgets;
public class Phidgetsample : MonoBehaviour {
	private InterfaceKit waterController;
	private checkHandPoseing checkHandScript;
	private bool isRightHand=false;
	// Use this for initialization
	void Start () {
		checkHandScript = GameObject.Find ("HandController").GetComponent<checkHandPoseing> ();
		waterController = new InterfaceKit ();
		waterController.open ();
		waterController.waitForAttachment (1000);
		waterController.outputs[7]=true;
		normalMode ();
	}
	
	// Update is called once per frame
	void Update () {
		int State = checkHandScript.getState ();
		isRightHand = checkHandScript.isRightCatching ();
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
		default:
			normalMode();
			break;
		}

	}
	
	/// <summary>
	/// Scrape mode
	/// </summary>
	void touchMode()
	{
		waterController.outputs[0]=true;
		waterController.outputs[1]=true;
		waterController.outputs[2]=false;
		waterController.outputs[3]=true;
		waterController.outputs[4]=true;
		waterController.outputs[5]=false;
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

	void OnApplicationQuit()//終了時処理
	{
		waterController.outputs[0]=false;
		waterController.outputs[1]=false;
		waterController.outputs[2]=false;
		waterController.outputs[3]=false;
		waterController.outputs[4]=false;
		waterController.outputs[5]=false;
		waterController.outputs[7]=false;
	}
	
}
