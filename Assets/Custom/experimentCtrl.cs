using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class experimentCtrl : MonoBehaviour
{
    public int randSeed;
	public int TrialNum;
	public int EventTrial;
    // Start is called before the first frame update
    void Awake()
    {

		TrialNum = 1;
		Random.InitState(randSeed);

		#if UNITY_EDITOR
			return;
		#endif
		SceneManager.LoadScene(1,LoadSceneMode.Additive);
		SceneManager.LoadScene(2,LoadSceneMode.Additive);
		SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));

    }
	void Update(){

		if (Input.GetKeyDown(KeyCode.End)){
			resetExperiment();
			Debug.Log("TrialEnd");
			if(TrialNum == EventTrial){
				SceneManager.LoadScene(2,LoadSceneMode.Additive);
			}
		}
	}
	private void resetExperiment(){
		SceneManager.UnloadScene(1);
		SceneManager.LoadScene(1,LoadSceneMode.Additive);
		TrialNum++;

	}
}
