using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSystem{

	public class PedestrianSystemManager : MonoBehaviour {


		public enum SpawnType
		{
			SPAWNER, // Spawn pedestrians with spawner object
			AUTO //Spawn pedestrians automatically with time
		}
		[Tooltip("(Spawner - Spawn when spawner script object enters the waypoint) - (Auto - Spawn automatically with time)")]
		public SpawnType spawnType;

		[Tooltip("Time after which one pedestrian is spawned")]
		[Header ("Auto Spawn Settings")]
		public float timeDelay = 3;
		float time = 0;

		[Tooltip("Waypoint Prefab - Pedestrian moves among waypoints")]
		[Header ("Prefab Links")]
		public GameObject waypoint;
		[Tooltip("Pedestrian Prefabs - These are the pedestrians that move - It will be chosen randomly if more than 1.")]
		public GameObject [] pedestrians;

		[Tooltip("Max Pedestrian Spawn Limit")]
		[Header ("Values")]
		public int maxSpawnLimit = 3;
		[Tooltip("Current total number of pedestrians spawned")]
		public int curPedestiansSpawned;

		[Tooltip("Waypoints list")]
		[Header ("Values Check! (Do not change it)")]
		public List <GameObject> waypointObjects;

		void Start(){
			//Set time equal to time Delay
			time = timeDelay;
		}

		//Only works in AUTO mode
		void Update(){

			if (spawnType == SpawnType.AUTO) {
			
				time -= Time.deltaTime;

				if (time <= 0) {
				
					time = timeDelay;

					InstantiateAutoPedestrian ();
				}
			}
		}

		//Instantiate Pedestrians when AUTO mode is set. Time Based
		void InstantiateAutoPedestrian(){

			int rand = Random.Range (0, waypointObjects.Count - 1);

			waypointObjects [rand].GetComponent<Waypoint> ().Instatiate_Pedestrian ();
		}

		//Return random Pedestrian Object
		public GameObject Get_Pedestrian(){

			int rand = (int) Mathf.Round(Random.Range (0, pedestrians.Length));
			 
			return pedestrians [rand];
		}

		//Check the limit and return true if you can spawn pedestrians and false if not.
		public bool CanSpawnPedi(){
		
			if (curPedestiansSpawned < maxSpawnLimit) {
		
				return true;

			} else {
			
				return false;
			}
		}



		//>>>>>>>>>>>>>>>>> CUSTON SCRIPT CALLED FUNCTIONS <<<<<<<<<<<<<<<<<<<<<<<<<<<<<//

		//Called from custom editor. Incase User Loses the link of waypoints. It assisns them back
		public void GetWaypoints(){

			waypointObjects.Clear ();
			int count = this.transform.childCount;
			GameObject [] tempObj = new GameObject[count];

			for (int i = 0; i < count; i++) {

				waypointObjects.Add(this.transform.GetChild (i).gameObject);
			}
		}

		//Called from custom editor. Adds a waypoint in Scene.
		public void AddWaypoint(){
		
			Transform temp = waypointObjects[ waypointObjects.Count - 1].transform;

			//Instantiating Waypoint and Setting Parent
			GameObject obj = (GameObject) Instantiate (waypoint, temp.position + (new Vector3(0,0,5)), temp.rotation);
			obj.transform.parent = this.transform;

			//Setting Name of Waypoint
			int i = waypointObjects.Count + 1;
			obj.gameObject.name = i.ToString ();

			//Getting Waypoint Components
			Waypoint waypointScript1 = obj.GetComponent<Waypoint> ();
			Waypoint waypointScript2 = waypointObjects[waypointObjects.Count - 1].GetComponent<Waypoint> ();

			//Setting Next Waypoints of Waypoints
			waypointScript1.nextWaypoint = waypointObjects [0].GetComponent<Waypoint> ();
			waypointScript2.nextWaypoint = waypointScript1;

			//Setting Manager Waypoint
			waypointScript1.manager = this;

			//Addinng Waypoint to the list
			waypointObjects.Add(obj);
		}

		//Called from custom editor. Adds last waypoint from Scene.
		public void RemoveLastWaypoint(){

			if (waypointObjects.Count == 2) {
			
				Debug.LogError ("Not allowed to destroy first two waypoints.");
			
			} else {

				DestroyImmediate (waypointObjects [waypointObjects.Count - 1]);
				waypointObjects.RemoveAt (waypointObjects.Count-1);

				waypointObjects [waypointObjects.Count-1].GetComponent<Waypoint> ().nextWaypoint = waypointObjects [0].GetComponent<Waypoint> ();
			}
		}


	}

}
