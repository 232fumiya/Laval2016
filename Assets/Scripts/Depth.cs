using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System;
public class Depth : MonoBehaviour {
	///<summary>
	/// point cloud
	/// 重い処理が多いので基本的にコルーチンで回し続ける。
	/// </summary>
	// FROM KINECT v2
	private KinectSensor _Sensor;
	//depth
	public GameObject depthSourceManager;
	DepthSourceManager depthSourceManagerScript;
	FrameDescription depthFrameDesc;
	private int depthWidth;
	private int depthHeight;
	private ushort[] rawdata;
	//mapper
	CoordinateMapper mapper;

	//color
	ColorFrameReader color_reader;
	FrameDescription colorFrameDesc;
	ColorSpacePoint[] colorSpacePoints;
	byte[] color_array;
	FrameDescription colorDesc;
	private bool CHECK_GETDATA=false;
	//間引き
	private int relief=20;
	//人物領域の数
	private int playerNumber;
	private int countPlayerIndexes=0;
	private bool beforeFindPlayerIndex=false;
	KinectPlayer playerScripts;
	//camera
	CameraSpacePoint[] cameraSpacePoints;

	//PlayerIndex
	CoordinateMapperManager coordinateMapperManagerScript;
	public GameObject coordinateMapperManager;
	private byte[] playerIndex;
	private int hitEnemyCount = 0;
	//enemyCollider
	private int[] EnemyNumber=new int[6];
	private bool  StartGetEnemyNum=false;
	BoxCollider[] enemiesCollider; 

	Vector3[] min = new Vector3[20];
	Vector3[] max = new Vector3[20];
	float[] midX = new float[20];
	float[] midY = new float[20];

	private int hitCounter=0;
	// PARTICLE SYSTEM
	private ParticleSystem.Particle[] particles;
	private ParticleSystem particle;
	private int enemyIndexCounts;
	public float size = 0.2f;
	public float scale = 10f;
	private bool[] hitEnemy = new bool[20];
	private bool MirrorMode=false;

	void Start () {
		enemiesCollider=GetComponents<BoxCollider>();
		playerScripts=GameObject.Find("Player").GetComponent<KinectPlayer>();
		_Sensor = KinectSensor.GetDefault ();
		if (_Sensor != null)
		{
			//デプスとカラーのデータ取得初期化処理
			depthFrameDesc = _Sensor.DepthFrameSource.FrameDescription;
			depthWidth = depthFrameDesc.Width;
			depthHeight = depthFrameDesc.Height;
			depthSourceManagerScript = depthSourceManager.GetComponent<DepthSourceManager> ();

			mapper = _Sensor.CoordinateMapper;

			cameraSpacePoints = new CameraSpacePoint[depthWidth * depthHeight];

			particles = new ParticleSystem.Particle[((depthWidth * depthHeight)/relief)+1];
			color_reader = _Sensor.ColorFrameSource.OpenReader ();
			colorFrameDesc = _Sensor.ColorFrameSource.CreateFrameDescription (ColorImageFormat.Rgba);
			colorSpacePoints = new ColorSpacePoint[depthWidth * depthHeight];
			color_array = new byte[colorFrameDesc.BytesPerPixel * colorFrameDesc.LengthInPixels];

			coordinateMapperManagerScript=coordinateMapperManager.GetComponent<CoordinateMapperManager>();
			playerIndex= new byte[colorFrameDesc.BytesPerPixel * colorFrameDesc.LengthInPixels];

			if (!_Sensor.IsOpen)
			{
				_Sensor.Open();
			}
		} 
		enemyIndexCounts = -1;
		particle = GetComponent<ParticleSystem> ();
		StartCoroutine ("enemy");
		//StartCoroutine ("checkDist");
	}
	private IEnumerator enemy()
	{
		while (true) {
			// get new depth data from DepthSourceManager.
			rawdata = depthSourceManagerScript.GetData ();
			if (rawdata != null) {
				//Debug.Log (rawdata.Length);
				if (color_reader == null)
					Debug.Log ("error");
				if (color_reader != null) {
					var Reference = color_reader.AcquireLatestFrame ();
					//Debug.Log(color_reader.AcquireLatestFrame());
					if (Reference != null) {
						Reference.CopyConvertedFrameDataToArray (color_array, ColorImageFormat.Rgba);
						CHECK_GETDATA = true;
						//Debug.Log(color_array.Length);
						Reference.Dispose ();
						Reference = null;
					}
				}
				playerIndex = coordinateMapperManagerScript.GetBodyIndexBuffer ();
				playerNumber=playerScripts.getPlayerNum();
				mapper.MapDepthFrameToCameraSpace (rawdata, cameraSpacePoints);
				mapper.MapDepthFrameToColorSpace (rawdata, colorSpacePoints);
				yield return null;
				//Debug.Log (colorSpacePoints.Length);
				StartGetEnemyNum=false;//getEnemyData()用
				for (int i=0; i<cameraSpacePoints.Length; i+=relief) {//100回に一回
					enemyIndexCounts++;
					if(enemyIndexCounts%500==0)
						yield return null;
					if (playerIndex [i] == 255 || rawdata [i] == 0 ||(playerIndex[i]==playerNumber && !MirrorMode)) {
						particles [enemyIndexCounts].position = new Vector3 (cameraSpacePoints [i].X * scale, cameraSpacePoints [i].Y * scale, cameraSpacePoints [i].Z * scale);
						particles [enemyIndexCounts].size = 0;
						particles [enemyIndexCounts].lifetime = -1;
						continue;
					}
					if((i-depthWidth-1)>0&&(i+depthWidth+1)<playerIndex.Length)
					{
						if(checkPlayerIndexesWrong(i))
						{
							particles [enemyIndexCounts].position = Vector3.zero;
							particles [enemyIndexCounts].size = 0;
							particles [enemyIndexCounts].lifetime = -1;
							continue;
						}
					}
					countPlayerIndexes++;
					float height=10;
					particles [enemyIndexCounts].position = new Vector3 (cameraSpacePoints [i].X * scale, cameraSpacePoints [i].Y * scale+height, cameraSpacePoints [i].Z * scale);
					particles [enemyIndexCounts].lifetime = 1;
					float pointSize=size+(particles[enemyIndexCounts].position.z/100);
					particles[enemyIndexCounts].size=pointSize;
					getEnemyData(playerIndex[i],particles[enemyIndexCounts].position);
					if(hitEnemy[playerIndex[i]])
					{
						particles [enemyIndexCounts].color = new Color32 (255,255,255,255);
					}
					else
					{
						long colorX = float.IsInfinity (colorSpacePoints [i].X) ? 0 : (int)Mathf.Floor (colorSpacePoints [i].X);
						long colorY = float.IsInfinity (colorSpacePoints [i].Y) ? 0 : (int)Mathf.Floor (colorSpacePoints [i].Y);
						if (colorX < 0)
							colorX = 0;
						if (colorY < 0)
							colorY = 0;
						long colorIndex = ((colorY * colorFrameDesc.Width) + colorX) * 4;
						if (CHECK_GETDATA == true) {
							if (colorIndex < 0) {
								Debug.Log ("error" + colorIndex);
							}
							if (colorIndex < color_array.Length) {
								byte r = color_array [colorIndex];
								byte g = color_array [colorIndex + 1];
								byte b = color_array [colorIndex + 2];
								byte alpha = 255;
								particles [enemyIndexCounts].color = new Color32 (r, g, b, alpha);
							}		
						}
					}
				}
				if (countPlayerIndexes != 0 || beforeFindPlayerIndex == true) {
					if (countPlayerIndexes != 0)
						beforeFindPlayerIndex = true;
					else
						beforeFindPlayerIndex = false;
					particle.SetParticles (particles, particles.Length);
					countPlayerIndexes = 0;
				} else {
					particle.Clear();
				}
				moveCollider();
				enemyIndexCounts = -1;
			}
			yield return null;
		}
	}
	private bool checkPlayerIndexesWrong(int playerNum)
	{
		int count=8;
		if(playerIndex [playerNum - depthWidth - 1] == 255)
			count--;
		if(playerIndex[playerNum-depthWidth]==255)
			count--;
		if(playerIndex[playerNum-depthWidth+1]==255)
			count--;
		if(playerIndex[playerNum-1]==255)
			count--;
		if(playerIndex[playerNum+1]==255)
			count--;
		if(playerIndex[playerNum+depthWidth-1]==255)
			count--;
		if(playerIndex[playerNum+depthWidth]==255)
			count--;
		if(playerIndex[playerNum+depthWidth+1]==255)
			count--;
		bool checkWrong = (count < 8);
		return checkWrong;
	}


	private void getEnemyData(int enemyIndexNum,Vector3 partPos)
	{
		if (StartGetEnemyNum == false) {
			for(int j=0; j<EnemyNumber.Length;j++)
			{
				EnemyNumber[j]=255;
				SetScaleData(j,Vector3.zero);
			}
			//ひとつ目のデータを格納
			EnemyNumber[0]=enemyIndexNum;
			SetScaleData(0,partPos);
			StartGetEnemyNum=true;
			return;
		}
		for(int i=0; i<EnemyNumber.Length;i++)
		{
			if(EnemyNumber[i]==255||(EnemyNumber[i]==playerNumber && !MirrorMode))
			{
				EnemyNumber[i]=enemyIndexNum;
				SetScaleData(i,partPos);
				return;
			}
			else if(EnemyNumber[i]==enemyIndexNum)
			{
				if(partPos.x<min[i].x)
					min[i].x = float.IsInfinity (partPos.x) ? min[i].x : partPos.x;
				else if(max[i].x<partPos.x)
					max[i].x = float.IsInfinity (partPos.x) ? max[i].x : partPos.x;
				if(partPos.y<min[i].y)
					min[i].y = float.IsInfinity (partPos.y) ? min[i].y : partPos.y;
				else if(max[i].y<partPos.y)
					max[i].y = float.IsInfinity (partPos.y) ? max[i].y: partPos.y;
				if(min[i].z<partPos.z)
					min[i].z = float.IsInfinity (partPos.z) ? min[i].z : partPos.z;
				break;
			}
		}
	}

	
	/// <summary>
	/// Sets the scale data.
	/// </summary>
	/// <param name="num">Number</param>
	/// <param name="pos">Position</param>
	private void SetScaleData(int num,Vector3 pos)
	{
		min[num].x = float.IsInfinity (pos.x) ? min[num].x : pos.x;
		max[num].x = float.IsInfinity (pos.x) ? max[num].x : pos.x;
		min[num].y = float.IsInfinity (pos.y) ? min[num].y : pos.y;
		max[num].y = float.IsInfinity (pos.y) ? max[num].y : pos.y;
		min[num].z = float.IsInfinity (pos.z) ? min[num].z : pos.z;
	}


	private void moveCollider()
	{
		for (int num=0; num<enemiesCollider.Length; num++) {
			Debug.Log(EnemyNumber[num]);
			if(EnemyNumber[num]==255||(EnemyNumber[num]==playerNumber && !MirrorMode))
			{
				enemiesCollider[num].center=new Vector3(0,-100,0);
				enemiesCollider[num].enabled=false;
				continue;
			}
			float scaleX = scaleCrate(max[num].x , min[num].x);
			float scaleY = scaleCrate(max[num].y , min[num].y);
			float scaleZ = 3f;
		 	midX[EnemyNumber[num]] =midCreate(max[num].x , min[num].x)/2;
			midY[EnemyNumber[num]] =midCreate(max[num].y,min[num].y)/2;//EnemyObjの高さと同じだけ引き算する必要あり
			float midZ=midCreate(min[num].z,0f)/2+(scaleZ);
			Vector3 centerPos=new Vector3(midX[EnemyNumber[num]],midY[EnemyNumber[num]],midZ);
			enemiesCollider[num].enabled=true;
			enemiesCollider[num].center=centerPos;
			enemiesCollider[num].size=new Vector3(scaleX,scaleY,scaleZ);
		}
	}
	private float midCreate(float max,float min)
	{
		float mid = 0f;
		if (0 < max) {
			if(min<0)
				min = -min;
			mid = max-min;
		}else{
			mid = min-max;
		}
		return mid;
	}
	private float scaleCrate(float max,float min)
	{
		if (min < 0)
			min = -min;
		if (max < 0)
			max = -max;
		float scale = max + min;
		return scale;
	}

	void OnApplicationQuit()
	{
		if (mapper != null)
		{
			mapper = null;
		}
		if (_Sensor != null)
		{
			if (_Sensor.IsOpen)
			{
				_Sensor.Close();
			}
			_Sensor = null;
		}
	}
	public void hitSnow(Vector3 hitPos)
	{
		hitCounter++;
		float beforeDist = 1000f;
		int mostNearEnemy = 0;
		for (int num=0; num<max.Length; num++) {
			if(EnemyNumber[num]==255||(EnemyNumber[num]==playerNumber&& !MirrorMode))
			{
				continue;
			}
			float dist =Vector2.Distance(new Vector2(midX[EnemyNumber[num]],midY[EnemyNumber[num]]),new Vector2(hitPos.x,hitPos.y));
			if(dist<beforeDist)
			{
				beforeDist=dist;
				mostNearEnemy=EnemyNumber[num];
			}
		}
		if (!hitEnemy [mostNearEnemy]) {
			hitEnemyCount++;
		}
			hitEnemy [mostNearEnemy] = true;
	}
	public int getHitCount()
	{
		return hitCounter;
	}
	public int getHitEnemies()
	{
		return hitEnemyCount;
	}
	public void reset(){
		hitCounter = 0;
		hitEnemyCount = 0;
		for (int i=0; i<6; i++) {
			hitEnemy[i]=false;
		}
	}
	public void setMirrorMode(bool isMirrorMode)
	{
		MirrorMode = isMirrorMode;
	}
}