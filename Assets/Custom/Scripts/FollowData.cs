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
	public bool Trolley;
	public List<string> LoadedInput;
    // Start is called before the first frame update
    void Start()
    {
		sr = new StreamReader(path+ fname);
		//Trolley = false;
		Auto.SetActive(true);
		Error.SetActive(false);
		LoadSaved();
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
	// Update is called once per frame
    void Update()
    {
		if(followIndex>=maxIndex) {
			Auto.SetActive(false);
			Error.SetActive(true);
			if(Trolley) {
				lsw.accel = 1;
				lsw.brake = 0;
				lsw.neutral = false;
				lsw.reverse = false;
			}
			return;
		}
		buffer = LoadedInput[followIndex];
		data = buffer.Split(',');
		followIndex++;
		targetPos = new Vector3(float.Parse(data[15]),float.Parse(data[16]),float.Parse(data[17]));
		targetEuler = new Vector3(float.Parse(data[18]),float.Parse(data[19]),float.Parse(data[20]));
		lsw.accel = float.Parse(data[3]);
		lsw.brake = float.Parse(data[4]);
		lsw.wheel = float.Parse(data[5]);
		lsw.neutral = bool.Parse(data[25]);
		lsw.reverse = bool.Parse(data[26]);
//		if(buffer == null) {
//			finish = true;
//			Debug.Log("Finished");
//			return;
//		}
//		//Vector3 pos = new Vector3 (float.Parse(data[12]),float.Parse(data[13]),float.Parse(data[14]));
//		//Vector3 lEuler = new Vector3 (float.Parse(data[15]),float.Parse(data[16]),float.Parse(data[17]));
//		if(float.Parse(data[21]) >= Time.time) {
//			
//		}
		car.position = Vector3.MoveTowards(car.position,targetPos,0.1f);
		car.rotation = Quaternion.RotateTowards(car.rotation,Quaternion.Euler(targetEuler),0.1f);//Vector3.RotateTowards(car.localEulerAngles,targetEuler,.1f,0.0f); //targetEuler;

	}
}
