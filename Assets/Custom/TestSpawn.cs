using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    public bool spawn;
    public GameObject spawnThis;
    public float speed;
    public float stopDist;
    public float safeDist;
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }
    void Spawn()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform spawnT = transform.GetChild(i).GetChild(0);
            GameObject spawnee = Instantiate(spawnThis, spawnT.position, spawnT.rotation);
            NodeFollower nf = spawnee.GetComponent<NodeFollower>();
            nf.node = spawnT.parent.GetComponent<Node>();
            nf.node.occupied++;
            nf.speed = speed;
            nf.stopDist = stopDist;
            nf.safeDist = safeDist;
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
