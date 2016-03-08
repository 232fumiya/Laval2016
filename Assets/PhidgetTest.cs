
using UnityEngine;
using System.Collections;
using Phidgets;
public class PhidgetTest : MonoBehaviour {
	private InterfaceKit waterController;
	// Use this for initialization
	void Start () {
		waterController = new InterfaceKit ();
		waterController.open ();
		waterController.waitForAttachment (1000);
		waterController.outputs[0]=true;
		waterController.outputs[1]=true;
		waterController.outputs[2]=true;
		waterController.outputs[3]=true;
		waterController.outputs[4]=true;
		waterController.outputs[5]=true;
		waterController.outputs[7]=true;
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	/// <summary>
	/// Scrape mode
	/// </summary>
	/*void touchMode()
	{
		waterController.outputs[0]=true;
		waterController.outputs[1]=true;
		waterController.outputs[2]=false;

	}
	/// <summary>
	/// Catchs the mode
	/// </summary>
	void catchMode()
	{
			waterController.outputs [0] = true;
			waterController.outputs [1] = false;
			waterController.outputs [2] = false;
	}
	/// <summary>
	/// Shoots the mode
	/// </summary>
	void shootMode()
	{
			waterController.outputs [0] = false;
			waterController.outputs [1] = true;
			waterController.outputs [2] = true;
	}
	void normalMode()
	{
			waterController.outputs [0] = true;
			waterController.outputs [1] = true;
			waterController.outputs [2] = true;
	}*/

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
