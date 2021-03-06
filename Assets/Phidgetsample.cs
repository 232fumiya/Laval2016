﻿using UnityEngine;
using System.Collections;
using Phidgets;
public class Phidgetsample : MonoBehaviour {
	private InterfaceKit waterController;
	private checkHandPoseing checkHandScript;
	private bool isRightHand=false;
	private bool isWaterControl=false;
	// Use this for initialization
	void Start () {
		if(Application.loadedLevelName=="Main")
			checkHandScript = GameObject.Find ("HandController").GetComponent<checkHandPoseing> ();
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
			int State = checkHandScript.getState ();
			isRightHand = checkHandScript.isRightCatching ();
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
