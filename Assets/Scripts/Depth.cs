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
	//enemyCollider
	private int[] EnemyNumber=new int[6];
	private bool  StartGetEnemyNum=false;
	BoxCollider[] enemiesCollider; 
	float[] minX = new float[6];
	float[] maxX = new float[6];
	float[] minY = new float[6];
	float[] maxY = new float[6];
	float[] minZ = new float[6];
	private Vector3 beforeCenterPos;
	int count=0;
	// PARTICLE SYSTEM
	private ParticleSystem.Particle[] particles;
	private ParticleSystem particle;
	private int counts;
	public float size = 0.2f;
	public float scale = 10f;
	private bool checkSnowHitIsPlaying=false;
	private float snowAndEnemyDist=100f;
	UnityEngine.AudioSource audio;

	void Start () {
		enemiesCollider=GetComponents<BoxCollider>();
		audio=this.GetComponent<UnityEngine.AudioSource>();
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
		counts = -1;
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
					counts++;
					if(counts%1000==0)
						yield return null;
					if (playerIndex [i] == 255 || rawdata [i] == 0 ||playerIndex[i]==playerNumber) {
						particles [counts].position = new Vector3 (cameraSpacePoints [i].X * scale, cameraSpacePoints [i].Y * scale, cameraSpacePoints [i].Z * scale);
						particles [counts].size = 0;
						particles [counts].lifetime = -1;
						continue;
					}
					if((i-depthWidth-1)>0&&(i+depthWidth+1)<playerIndex.Length)
					{
						if(checkPlayerIndexesWrong(i))
						{
							particles [counts].position = Vector3.zero;
							particles [counts].size = 0;
							particles [counts].lifetime = -1;
							continue;
						}
					}
					countPlayerIndexes++;
					float height=10;
					particles [counts].position = new Vector3 (cameraSpacePoints [i].X * scale, cameraSpacePoints [i].Y * scale+height, cameraSpacePoints [i].Z * scale);
					particles [counts].lifetime = 1;
					float pointSize=size+(particles[counts].position.z/100);
					particles[counts].size=pointSize;
					getEnemyData(playerIndex[i],particles[counts].position);

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
							particles [counts].color = new Color32 (r, g, b, alpha);
						}
					}
				}
				if (countPlayerIndexes != 0 || beforeFindPlayerIndex == true) {
					if (countPlayerIndexes != 0)
						beforeFindPlayerIndex = true;
					else
						beforeFindPlayerIndex = false;
					particle.SetParticles (particles, particles.Length);
					moveCollider();
					yield return null;
					countPlayerIndexes = 0;
				} else {
					particle.Clear();
					audio.Stop();
				}
				counts = -1;
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
			if(EnemyNumber[i]==255||EnemyNumber[i]==playerNumber)
			{
				EnemyNumber[i]=enemyIndexNum;
				SetScaleData(i,partPos);
				return;
			}
			else if(EnemyNumber[i]==enemyIndexNum)
			{
				if(partPos.x<minX[i])
					minX [i] = float.IsInfinity (partPos.x) ? minX[i] : partPos.x;
				else if(maxX[i]<partPos.x)
					maxX [i] = float.IsInfinity (partPos.x) ? maxX[i] : partPos.x;
				if(partPos.y<minY[i])
					minY [i] = float.IsInfinity (partPos.y) ? minY[i] : partPos.y;
				else if(maxY[i]<partPos.y)
					maxY [i] = float.IsInfinity (partPos.y) ? maxY[i] : partPos.y;
				if(minZ[i]<partPos.z)
					minZ [i] = float.IsInfinity (partPos.z) ? minZ[i] : partPos.z;
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
		minX [num] = float.IsInfinity (pos.x) ? minX[num] : pos.x;
		maxX [num] = float.IsInfinity (pos.x) ? maxX[num] : pos.x;
		minY [num] = float.IsInfinity (pos.y) ? minY[num] : pos.y;
		maxY [num] = float.IsInfinity (pos.y) ? maxY[num] : pos.y;
		minZ [num] = float.IsInfinity (pos.z) ? minZ[num] : pos.z;
	}


	private void moveCollider()
	{
		for (int num=0; num<enemiesCollider.Length; num++) {
			if(EnemyNumber[num]==255||EnemyNumber[num]==playerNumber)
			{
				enemiesCollider[num].enabled=false;
				continue;
			}
			float scaleX = scaleCrate(maxX[num] , minX[num]);
			float scaleY = scaleCrate(maxY[num] , minY[num]);
			float scaleZ = 3f;
		 	float midX =midCreate(maxX[num] , minX[num])/2;
			float midY =midCreate(maxY[num],minY[num])/2-10;//EnemyObjの高さと同じだけ引き算する必要あり
			float midZ=midCreate(minZ[num],0f)/2+(scaleZ);
			Vector3 centerPos=new Vector3(midX,midY,midZ);
			enemiesCollider[num].enabled=true;
			enemiesCollider[num].center=centerPos;
			enemiesCollider[num].size=new Vector3(scaleX,scaleY,scaleZ);
			float dist=Vector3.Distance(beforeCenterPos,centerPos);
			if(!audio.isPlaying && 5f<dist){
				audio.Play();
			}
			else if(audio.isPlaying && dist < 5f)
			{
				if(10<count){
					audio.Pause();
					count=0;
				}
				count++;
			}else{
				count=0;
			}
			beforeCenterPos=centerPos;
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
}