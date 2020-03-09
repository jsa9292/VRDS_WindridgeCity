using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    public GameObject spawnThis;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++) {
            Transform spawnT = transform.GetChild(i).GetChild(0);
            GameObject spawnee = Instantiate(spawnThis, spawnT.position,spawnT.rotation);
            NodeFollower nf = spawnee.GetComponent<NodeFollower>();
            nf.node = spawnT.parent.GetComponent<Node>();
            nf.speed = 0.1f;
            nf.stopDist = 0.2f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
