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
    void Start()
    {
        ped = peds[Random.Range(0, peds.Count)];
        ped = Transform.Instantiate(ped, transform.position, transform.rotation,transform.parent);
        pAI = ped.GetComponent<pedAI>();
        pAI.nf = transform.GetComponent<NodeFollower>();
    }

}
