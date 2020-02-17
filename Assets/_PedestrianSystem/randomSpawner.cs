using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class randomSpawner : MonoBehaviour
{

    public Transform target;
    public float spawnTime = 1f;
	public float spawnRate = 20f;
    public Transform[] spawnPoints;
    // public GameObject soccerMom;
    public List<GameObject> Pedestrians; // list of pedestrian prefabs
    // public GameObject suburbanMan;
    int x;


    // Use this for initialization
    void Start()
    {
        InvokeRepeating("Spawn", spawnTime, spawnRate);
    }

    void Spawn()
    {

        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
		int pedestrianIndex = Random.Range (0, 2);
		//one way is to have bunch of if statements to have partitions
		//Mathf.CeilToInt ();
		//Mathf.FloorToInt ();
		//if the input of the Random.Range is integer it will produce integers.
		//if the input is float, it will produce a random float
        x = transform.childCount;



        if (x < 3)
        {
            GameObject g = Instantiate(Pedestrians[pedestrianIndex], spawnPoints[spawnPointIndex].position, transform.rotation, transform);
            // GameObject h=Instantiate(suburbanMan, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, transform);
            PedestrianMove pMove = g.GetComponent<PedestrianMove>();
            // PedestrianMove manMove = h.GetComponent<PedestrianMove>();
            pMove.target = target;

            // manMove.target = target;
            // manMove.rotationSpeed = 0.5f;
        }

    }


    // Update is called once per frame
    void Update()
    {

    }
}