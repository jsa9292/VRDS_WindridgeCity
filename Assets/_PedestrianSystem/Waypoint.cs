using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {
    int n;
    public Color color;
    public Transform firstWaypoint;
    public Transform lastWaypoint;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void OnDrawGizmos () {
        n = transform.childCount;

        for (int i = 0; i < n; i++) {
           // Vector3 firstWaypoint = transform.GetChild(i).position;
           // Vector3 lastWaypoint = Vector3.zero;

            if (i + 1 < n)
            {
                Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
            }
            
        }

        Gizmos.DrawLine(transform.GetChild(0).position,transform.GetChild(n-1).position);
    }


}
