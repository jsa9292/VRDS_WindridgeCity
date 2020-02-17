using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomWaypointMove : MonoBehaviour {

    public float timer;
    public Transform[] waypoints;
    public int destPoint = 0;

    
	// Use this for initialization
	void Start () {

        timer += Time.deltaTime;

    }


    // Update is called once per frame
    void Update () {
    }
}
