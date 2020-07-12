using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedSpawner : MonoBehaviour
{
    public List<GameObject> peds;
    public float moveCoeff;
    private GameObject ped;
    private Animator anim;
    private NodeFollower nf;
	public int randseed =0;
	public System.Random rand;
    // Start is called before the first frame update
	void Awake()
	{
		rand = new System.Random(randseed);

	}
    void Start()
    {
        ped = peds[rand.Next(0, peds.Count)];
        ped = Transform.Instantiate(ped, transform.position, transform.rotation, transform);
        anim = ped.GetComponent<Animator>();
        nf = ped.transform.parent.GetComponent<NodeFollower>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        anim.SetFloat("Speed", nf.movement * moveCoeff);
    }
}
