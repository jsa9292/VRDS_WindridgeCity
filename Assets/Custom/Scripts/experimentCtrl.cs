using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class experimentCtrl : MonoBehaviour
{
    public int randseed;
	public System.Random rand;
	public int TrialNum;
	public int EventTrial;
	public int TargetFps;
	public FollowData fd;
	private Vector3 initialPos;
	private Quaternion initialRot;
    // Start is called before the first frame update
    void Awake()
	{
		//DontDestroyOnLoad(this.gameObject);
		Random.InitState(randseed); //Unity RNG init (global RNG)
		rand = new System.Random(randseed);//System RNG init (object RNG)
		Application.targetFrameRate = (int) TargetFps;
		initialPos= transform.position;
		initialRot= transform.rotation;
		#if UNITY_EDITOR
			return;
		#endif
		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
    }
	void Start(){
	}
	void Update(){
		if (Input.GetKeyDown(KeyCode.End)){
			resetTransform();
			TrialNum++;

//			if(TrialNum == EventTrial){
//				SceneManager.LoadScene(1,LoadSceneMode.Additive);
//
//				fd.enabled = true;
//				fd.Trolley = true;
//			}else{
//				fd.enabled = false;
//				fd.Trolley = false;
//			}
		}

	}
	void resetTransform(){
		transform.position=initialPos;
		transform.rotation=initialRot;
		SceneManager.LoadScene(0,LoadSceneMode.Single);
	}
}
