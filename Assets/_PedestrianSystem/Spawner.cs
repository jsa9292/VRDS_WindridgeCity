using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public Transform target;
    public float spawnTime = 10f;
    public Transform[] spawnPoints;
    public GameObject soccerMom;
    public List<GameObject> Pedestrians;
    // public GameObject suburbanMan;
    int x;


	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }

    void Spawn()
    {

        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        x = transform.childCount;

       

        if ( x < 5)
        {
            GameObject g=Instantiate(soccerMom, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, transform);
           // GameObject h=Instantiate(suburbanMan, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, transform);
            PedestrianMove pMove = g.GetComponent<PedestrianMove>();
           // PedestrianMove manMove = h.GetComponent<PedestrianMove>();
            pMove.target = target;
            pMove.rotationSpeed = 1.5f;

           // manMove.target = target;
           // manMove.rotationSpeed = 0.5f;
        }

        
    }


    // Update is called once per frame
    void Update () {
		
	}
}
