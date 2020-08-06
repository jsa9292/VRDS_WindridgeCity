using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FollowData : MonoBehaviour
{
	public Transform car;
	public string path;
	public string fname;
	private StreamReader sr;
	public LogitechSteeringWheel lsw;
	public bool Finished;
	public List<string> LoadedInput;
    // Start is called before the first frame update
    void Start()
    {
		sr = new StreamReader(path+ fname);
		Auto.SetActive(true);
		Error.SetActive(false);
		LoadSaved();
		buffer = LoadedInput[followIndex];
		data = buffer.Split(',');
		targetPos = new Vector3(float.Parse(data[15]),float.Parse(data[16]),float.Parse(data[17]));
		targetEuler = new Vector3(float.Parse(data[18]),float.Parse(data[19]),float.Parse(data[20]));
		car.position = targetPos;
		car.rotation = Quaternion.Euler(targetEuler);
    }
	void LoadSaved(){
		while(sr.Peek()> -1){
			buffer = sr.ReadLine();
			LoadedInput.Add(buffer);
		}
		maxIndex=LoadedInput.Count;
	}
	private int followIndex = 0;
	private int maxIndex;
	private string buffer;
	private string[] data;
	public GameObject Auto;
	public GameObject Error;
	public Vector3 targetPos;
	public Vector3 targetEuler;
	public float timeStep;
	// Update is called once per frame
    void Update()
    {
		if(followIndex>=maxIndex) {
			Auto.SetActive(false);
			Error.SetActive(true);
			Finished = true;
			this.enabled = false;
			return;
		}
		buffer = LoadedInput[followIndex];
		data = buffer.Split(',');
		followIndex++;
		timeStep = float.Parse(data[21]);
		lsw.accel = float.Parse(data[3]);
		lsw.brake = float.Parse(data[4]);
		lsw.wheel = float.Parse(data[5]);
		lsw.neutral = bool.Parse(data[25]);
		lsw.reverse = bool.Parse(data[26]);
		//micro adjustment for syncing pos and rot
		targetPos = new Vector3(float.Parse(data[15]),float.Parse(data[16]),float.Parse(data[17]));
		targetEuler = new Vector3(float.Parse(data[18]),float.Parse(data[19]),float.Parse(data[20]));
		car.position = Vector3.MoveTowards(car.position,targetPos,0.1f);
		car.rotation = Quaternion.RotateTowards(car.rotation,Quaternion.Euler(targetEuler),0.1f);//Vector3.RotateTowards(car.localEulerAngles,targetEuler,.1f,0.0f); //targetEuler;

	}
}
