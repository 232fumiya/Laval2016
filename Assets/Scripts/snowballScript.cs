using UnityEngine;
using System.Collections;

public class snowballScript : MonoBehaviour {
	// Use this for initialization
	private bool isShoot=false;
	private KinectPlayer playerScripts;
	private GameObject hitEffect;
	void Awake () {
		hitEffect = Resources.Load ("hitEffect") as GameObject;
		playerScripts=GameObject.Find("Player").GetComponent<KinectPlayer>();		
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
	void OnTriggerEnter(Collider other)
	{

		if (other.tag == "stage" && isShoot) {
			Destroy (this.gameObject);
		} else if (other.tag == "Player"&&!isShoot) {
			playerScripts.getTouch();
		} else if (other.tag == "enemy"&&isShoot) {
			GameObject effect =Instantiate(hitEffect,this.transform.position,Quaternion.identity) as GameObject;
			Destroy(this.gameObject);
		}

	}

}