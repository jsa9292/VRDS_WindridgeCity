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
    // Start is called before the first frame update
    void Start()
    {
		Physics.autoSimulation = false;
		QualitySettings.vSyncCount = 1;
		QualitySettings.maxQueuedFrames = 0;
		Application.targetFrameRate = 90;
		sr = new StreamReader(path+ fname);
    }

    // Update is called once per frame
    void Update()
    {
		string buffer = sr.ReadLine();
		string[] data = buffer.Split(',');
		Vector3 pos = new Vector3 (float.Parse(data[12]),float.Parse(data[13]),float.Parse(data[14]));
		Vector3 lEuler = new Vector3 (float.Parse(data[15]),float.Parse(data[16]),float.Parse(data[17]));
		Debug.Log(float.Parse(data[0]));
		car.position = pos;
		car.localEulerAngles = lEuler;
    }
}
