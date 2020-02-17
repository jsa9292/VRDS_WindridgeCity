using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSystem{

	public class Waypoint : MonoBehaviour {

		[Tooltip("Pedestrian System Manager Reference")]
		public PedestrianSystemManager manager;

		[Tooltip("Next waypoint Reference. Target will move to this point once hit this waypoint")]
		public Waypoint nextWaypoint;


		//Instantiate a pedestrian at this waypoint
		public void Instatiate_Pedestrian(){
		
			if (manager.CanSpawnPedi ()) {
			
				//instantiate pedestrian
				GameObject pedi = (GameObject)Instantiate (manager.Get_Pedestrian (), this.transform.position, this.transform.rotation);

				//assign target to pedestrian. It will moove toward its target
				pedi.GetComponent<Pedestrian> ().target = GetNextWaypoint ();

				//increment spawned pedestrians
				manager.curPedestiansSpawned++;

			} else {
			
				Debug.Log ("FULL");
			}
		}

		//return the transform of next waypoint
		public Transform GetNextWaypoint(){
		
			if (nextWaypoint)
				return this.nextWaypoint.transform;
			else
				return null;
		}
	}
}