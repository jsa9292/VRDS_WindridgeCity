using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedSpawner : MonoBehaviour
{
    public List<GameObject> peds;
    private GameObject ped;
    private pedAI pAI;
	public int randseed =0;
    // Start is called before the first frame update
	void Awake(){
		ped = peds[Random.Range(0, peds.Count)];


	}
	public void Spawn(Transform spawneeParent)
    {
		ped = Transform.Instantiate(ped, transform.position, transform.rotation,spawneeParent);
        pAI = ped.GetComponent<pedAI>();
        pAI.nf = transform.GetComponent<NodeFollower>();
    }

}
