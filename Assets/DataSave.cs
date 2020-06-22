using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class DataSave : MonoBehaviour {
	public LogitechSteeringWheel lsw;
	public unityDrivingLatency ud;
	public Transform hmd;
	public string filepath;
	public string filename;
	public Rigidbody Car;
	StreamWriter w;
	public bool end = false;
	public bool write = false;
	public float time;
	private bool newfile= true;
	public int randomSeed;
	// Use this for initialization
	void Awake(){
		filename=DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";
		UnityEngine.Random.InitState(randomSeed);
	}
	void Start () {
		time = 0;
	}

	// Update is called once per frame
	void LateUpdate () {
		write = !lsw.neutral;
		time += Time.deltaTime;
		if (write) {
			if (newfile) {
				w = new StreamWriter (filepath+filename);
				newfile = false;
			}
			w.WriteLine (
				ud.CurrentSpeed_z.ToString("0.0000") + "," +
				ud.CurrentAccel_z.ToString ("0.0000") + "," + 		//9
				ud.angularVelocity.ToString ("0.0000")+ "," +		//10
				lsw.accel.ToString ("0.0000") + "," + 				//0
				lsw.brake.ToString ("0.0000") + "," + 				//1
				lsw.wheel.ToString ("0.0000") + "," +				//2
				hmd.localEulerAngles.x.ToString ("0.0000") + "," + 	//3
				hmd.localEulerAngles.y.ToString ("0.0000") + "," + 	//4
				hmd.localEulerAngles.z.ToString ("0.0000") + "," +	//5
				ud.pitch.ToString ("0.0000") + "," + 				//6
				ud.roll.ToString ("0.0000") + "," + 				//7
				ud.yaw.ToString ("0.0000") + "," +					//8
				Car.transform.position.x.ToString ("0.0000") + "," +
				Car.transform.position.y.ToString ("0.0000") + "," +
				Car.transform.position.z.ToString ("0.0000") + "," +//8
				Car.transform.localEulerAngles.x.ToString ("0.0000") + "," +
				Car.transform.localEulerAngles.y.ToString ("0.0000") + "," +
				Car.transform.localEulerAngles.z.ToString ("0.0000") + "," +//8
				Time.time.ToString ("0.0000")							//11
			);
			w.Flush ();
		}
		if (end)
			w.Close ();
 	}
}
