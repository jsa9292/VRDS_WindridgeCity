using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSystem{

	public class PedestrianSpawner : MonoBehaviour {

		[Tooltip("Should Spawner instantiate pedestrian when its Trigger enters the waypoint Trigger")]
		[Header("Checks")]
		public bool instantiatePediOnEnter = true;
		[Tooltip("Should Spawner Destroy pedestrian when its Trigger exits the Pedestrian Trigger")]
		public bool destroyPediOnExit = true;
		[Tooltip("Should Spawner spawn new pedestrian after it destroyes one on waypoints that are in it or not")]
		public bool afterKillSpawnPedi = true;

		[Header("Refrences")]
		public PedestrianSystemManager pediSystemManager;

		//lists of Waypoints which are in Trigger of Spawner! Do not change it 
		[HideInInspector]
		public List<Waypoint> inTriggerWaypoints;

		//instantiate pedestrian on random waypoint when pedestian destroys and It can spawn
		void InstantiatePediOnRandomInTriggerWaypoint(){

			if (inTriggerWaypoints.Count > 0) {
			
				int rand = Random.Range (0, inTriggerWaypoints.Count - 1);

				inTriggerWaypoints [rand].Instatiate_Pedestrian ();
			}

		}

		//if enter the trigger and bool is checked. Then instantiate the pedestrian
		public void OnTriggerEnter(Collider col){
			
			if(col.CompareTag("Waypoint")){

				if (instantiatePediOnEnter) {
					
					col.GetComponent<Waypoint> ().Instatiate_Pedestrian ();
				}

				inTriggerWaypoints.Add (col.GetComponent<Waypoint> ());
			}

		}

		//if pedestrian exits from trigger then destroy it and spawn on random trigger
		public void OnTriggerExit(Collider col){

			if (col.CompareTag ("Waypoint")) {

				inTriggerWaypoints.Remove (col.GetComponent<Waypoint> ());
			}
			
			if(col.CompareTag("Pedestrian")){
				
				if (destroyPediOnExit) {
					
					col.GetComponent<Pedestrian> ().DestroyPedestrian (pediSystemManager);

					if(afterKillSpawnPedi)
						Invoke ("InstantiatePediOnRandomInTriggerWaypoint", 1);
				}
			}
		}
	}

}