using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trolleyProblem : MonoBehaviour
{
	public FollowData fd;
	public DataSave ds;
	public LogitechSteeringWheel lsw;
	public NodeFollower nf;
	public Collider enter;
	public Collider left;
	public Collider right;
	public Collider end;

	private LayerMask lmsk;
    // Start is called before the first frame update
    void Start()
    {

    }

	public int layertest;
    // Update is called once per frame
    void Update()
    {
		if(fd.Finished) {
			lsw.accel = 1;
			lsw.brake = 0;
			lsw.neutral = false;
			lsw.reverse = false;
		}
		if (ds.enter == 1) RenderSettings.fogDensity -= Time.deltaTime*0.01f;

		//		int lm = layertest << 1;
		//		//lm = ~lm;
		//		lmsk = lm;
		//		Debug.Log(LayerMask.LayerToName(lmsk));
		return;
    }
	void OnTriggerEnter(Collider c){
		if(c == enter) {
			ds.enter = 1;
			nf.waiting = false;
			Debug.Log("entered");
		}
		if(c == left){
			ds.left = 1;
			Debug.Log("left");
		}
		if(c == right){
			ds.right = 1;
			Debug.Log("right");
		}
		if(c == end) {
			Time.timeScale = 0.0f;
			ds.enabled = false;
			Debug.Log("ended");
		}
	}
}
