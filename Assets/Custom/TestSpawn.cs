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
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }
    void Spawn()
    {
        Transform spawnT;
        GameObject spawnee;
        NodeFollower nf;
		CarSpawner cs;
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnT = transform.GetChild(i).GetChild(0);
			spawnee = Instantiate(spawnThis,spawnParent);
            if (nf = spawnee.GetComponent<NodeFollower>())
            {
                nf.Setup(spawnT.parent.GetComponent<Node>());
            }
			if (cs = spawnee.GetComponent<CarSpawner>()){
				cs.Spawn(spawnParent);
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
