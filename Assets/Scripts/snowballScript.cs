using UnityEngine;
using System.Collections;

public class snowballScript : MonoBehaviour {
	// Use this for initialization
	private bool isShoot=false;
	private int snowNum;
	private GameObject HandsControl;
	//private checkHandPoseing handsScripts;
	private GameObject hitEffect;
	void Awake () {
		hitEffect = Resources.Load ("hitEffect") as GameObject;
	//	HandsControl = GameObject.Find ("HandController");
	//	handsScripts=HandsControl.GetComponent<checkHandPoseing>();

	}
	
	// Update is called once per frame
	void Update () {

	}
	public void handCenterPos(Vector3 centerPos)
	{
		if (centerPos.y < 0)
			centerPos.y = -centerPos.y;
		this.transform.position = centerPos;
	}
	public void changeSnowSize(Vector3 snowSize)
	{
		this.transform.localScale = snowSize;
	}
	public void isShooting(bool shoot)
	{
		isShoot = true;
	}
	public bool getShoot()
	{
		return isShoot;
	}
	public void sendSnowNum(int num)
	{
		snowNum = num;
	}
	public int snowNumber()
	{
		return snowNum;
	}
	void OnTriggerEnter(Collider other)
	{

		if (other.tag == "stage" && isShoot) {
			Destroy (this.gameObject);
		} else if (other.tag == "Player"&&!isShoot) {
//			handsScripts.touchSnow (true);
		} else if (other.tag == "enemy"&&isShoot) {
			GameObject effect =Instantiate(hitEffect,this.transform.position,Quaternion.identity) as GameObject;
			Destroy(this.gameObject);
		}

	}

}