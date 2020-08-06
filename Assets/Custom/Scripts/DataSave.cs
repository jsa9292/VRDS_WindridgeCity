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
	private bool newfile= true;
	private int entryIndex;
	public int enter = 0;
	public int left = 0;
	public int right = 0;
	// Use this for initialization
	void Awake(){
		#if UNITY_STANDALONE
		filename=DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
		entryIndex = 0;
		#endif
	}

	// Update is called once per frame
	void LateUpdate () {
		#if UNITY_STANDALONE
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
				ud.pitch.ToString ("0.0000") + "," + 				//6
				ud.roll.ToString ("0.0000") + "," + 				//7
				ud.yaw.ToString ("0.0000") + "," +					//8
				hmd.localPosition.x.ToString ("0.0000") + "," + 	//3
				hmd.localPosition.y.ToString ("0.0000") + "," + 	//4
				hmd.localPosition.z.ToString ("0.0000") + "," +	//5
				hmd.localEulerAngles.x.ToString ("0.0000") + "," + 	//3
				hmd.localEulerAngles.y.ToString ("0.0000") + "," + 	//4
				hmd.localEulerAngles.z.ToString ("0.0000") + "," +	//5
				Car.transform.position.x.ToString ("0.0000") + "," +
				Car.transform.position.y.ToString ("0.0000") + "," +
				Car.transform.position.z.ToString ("0.0000") + "," +//8
				Car.transform.localEulerAngles.x.ToString ("0.0000") + "," +
				Car.transform.localEulerAngles.y.ToString ("0.0000") + "," +
				Car.transform.localEulerAngles.z.ToString ("0.0000") + "," +//8
				Time.deltaTime.ToString ("0.0000") + "," +							//11
				enter.ToString() + "," + 
				left.ToString() + "," + 
				right.ToString() + "," + 
				lsw.neutral.ToString() + "," +
				lsw.reverse.ToString() + "," +
				entryIndex

			);
			w.Flush ();
			entryIndex++;
		}
		if (end)
			w.Close ();
		#endif
	}

}
