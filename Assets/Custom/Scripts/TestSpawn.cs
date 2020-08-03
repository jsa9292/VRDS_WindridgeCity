using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    public bool spawn;
    public GameObject spawnThis;
	public Transform spawnParent;
    public float speed;
    public float stopDist;
    public float safeDist;
    public float waitTime;
	public double spawnThreshold;
	public int randseed = 0;
	public System.Random rand;
    // Start is called before the first frame update
	void Awake(){
		rand = new System.Random(randseed);
		Spawn();
	}

    void Spawn()
    {
        Transform spawnT;
        GameObject spawnee;
        NodeFollower nf;
		CarSpawner cs;
		PedSpawner ps;
        for (int i = 0; i < transform.childCount; i++)
        {
			if(rand.NextDouble() > spawnThreshold) continue; 
            spawnT = transform.GetChild(i).GetChild(0);
			spawnee = Instantiate(spawnThis,spawnParent);
            if (nf = spawnee.GetComponent<NodeFollower>())
            {
                nf.Setup(spawnT.parent.GetComponent<Node>());
            }
			if (cs = spawnee.GetComponent<CarSpawner>()){
				cs.Spawn(spawnParent);
			}
			else if (ps = spawnee.GetComponent<PedSpawner>()){
				ps.Spawn(spawnParent);
			}
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (spawn) {
            Spawn();
            spawn = false;
        }
    }
}
