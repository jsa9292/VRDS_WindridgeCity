using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PedestrianSystem{

	public class Pedestrian : MonoBehaviour {

		public enum MovementType
		{
			WALK, // will walk
			RUN // will run
		}
		[Tooltip("Movement type of the pedestrian")]
		public MovementType movementType;

		[Tooltip("Target to which player will move and face towards")]
		public Transform target;

		[Tooltip("Pedestrian will walk with this speed")]
		[Header("Values")]
		public float walkSpeed = 0.2f;
		[Tooltip("Pedestrian will run with this speed")]
		public float runSpeed = 0.2f;
		[Tooltip("Pedestrian will rotate with this speed")]
		public float rotationSpeed = 1;

		bool isDestroyed = false;

		//set animator value of pedestrian according to the state choosen
		void Start(){

			Animator anim = this.GetComponent<Animator> ();

			switch (movementType) {

			case MovementType.WALK:

				anim.SetInteger ("MoveState", 1);

				return;

			case MovementType.RUN:

				anim.SetInteger ("MoveState", 2);

				return;
			}

		}

		void Update(){

			PedestrianMovement ();

			// if target is assinged. Rotate toward it accordingly
			if (target) {
			
				Quaternion targetRotation = Quaternion.LookRotation (target.position - this.transform.position, this.transform.up);
				targetRotation.x = 0; targetRotation.z = 0;
				this.transform.rotation = Quaternion.Lerp (this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			}
		}

		//movement acfording to movement type
		void PedestrianMovement(){

			switch (movementType) {

			case MovementType.WALK:

				this.transform.position += this.transform.forward * Time.deltaTime * walkSpeed;

				return;

			case MovementType.RUN:

				this.transform.position += this.transform.forward * Time.deltaTime * runSpeed;

				return;
			}
		}

		//properly destroy current pedestrian
		public void DestroyPedestrian(PedestrianSystemManager pedestrianSystem){
		
			if (!isDestroyed) {
			
				isDestroyed = true;

				pedestrianSystem.curPedestiansSpawned--;
				Destroy (this.gameObject);
			}
		}

		//move to next waypoint once hit the target waypoint
		public void OnTriggerEnter(Collider col){

			if(col.CompareTag("Waypoint")){

				if(col.gameObject == target.gameObject)
					target = col.GetComponent<Waypoint> ().GetNextWaypoint ();
			}
		}

	}
}
