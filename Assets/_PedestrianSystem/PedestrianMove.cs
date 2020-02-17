using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianMove : MonoBehaviour
{

    public float accel;
    public float brake;
    public float speed;
    //public Transform pedestrian;
    public List<Transform> Waypoints;
    // public GameObject suburbanMan;
    public Transform target;
    public float distance;
    public float rotationSpeed;
	public float offsetDistance;

	public float rayCast;
	public bool detect = true;
	RaycastHit hit;
	public bool rayhit;
	public float delta_Euler;

    Vector3 movement;
    public Animator anim;


    // Use this for initialization
    void Start()
    {
   
    }


    // Update is called once per frame
    void Update()
    {

		if (detect)
		{
			if (rayhit = Physics.Raycast(transform.position+transform.up*1.2f, target.position-transform.position,out hit, rayCast))
				{
					if (hit.collider.tag == "Pedestrian")
					{
						Debug.Log("hit");
					}
				}
				Debug.DrawRay(transform.position + transform.up * 1f, transform.forward * rayCast);
		}
				Debug.DrawRay(transform.position + transform.up * 1f, transform.forward * rayCast, Color.yellow);
       
		if (Vector3.Distance(transform.position, target.position) < distance)
        {
            // if target waypoint has component, now pedestrian has new target
            if (target.GetComponent<RandomWaypointMove>())
            {
                RandomWaypointMove rwm = target.GetComponent<RandomWaypointMove>();
                //gameObject.transform.GetComponent<RandomWaypointMove>();
                //if (rwm.waypoints.Length > 0) target = rwm.waypoints[0]; 

				if (rwm.waypoints.Length > 0 && Random.Range(0.0f,1.0f)>0.9f)
                {
                    int waypointIndex = Random.Range(0, rwm.waypoints.Length); //creating index for potential waypoint options //**FRIDAY** need to assign % chance to the random range, look up random number generator
                    target = rwm.waypoints[waypointIndex];
                }
                else
                {
                    int i = target.GetSiblingIndex();
                    if (i + 1 < target.transform.parent.childCount) target = target.transform.parent.GetChild(i + 1);
                    if (i + 1 == target.transform.parent.childCount) target = target.transform.parent.GetChild(0);
                }

            }

            //accel = 1f; //distance / 1.5f - 0.4f;
            
            // Pedestrian Moves to other side of block - do I need this? 
            // if (i + 1 == target.transform.parent.childCount) target = target.transform.parent.GetChild(0);
        }

        // How do I get this to "restart"??? 

        // if (distance < 1.6f) // don't need to stop right now
        //    accel = 0; 

		if (Physics.Raycast(transform.position+Vector3.up*.5f, -transform.up, out hit))
		{
			//Debug.Log("Raycast hit object" + hit.distance);
			//Debug.Log("Distance between object and " + hit.transform.name + ":" + hit.distance);
			transform.position -= new Vector3 (0f, hit.distance-offsetDistance, 0f);
			//Vector3 targetLocation = hit.point;
			//targetLocation += new Vector3 (0, transform.localScale.y / 2, 0);
			//transform.position = targetLocation;
			Debug.DrawRay (transform.position+Vector3.up*.5f, -transform.up, Color.red);
		}

        transform.position += transform.forward * accel * Time.deltaTime;
        //float relativePos = Vector3.SignedAngle(transform.forward, target.position - transform.position,transform.up);
		delta_Euler = Vector3.SignedAngle(transform.forward, target.position + transform.right * System.Convert.ToInt32(rayhit) - transform.position, transform.up);
        Vector3 target_Euler = transform.localEulerAngles + transform.up * -delta_Euler;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(target_Euler) ,  Mathf.Clamp(Mathf.Abs(delta_Euler / rotationSpeed), 0f, 50f));
        //transform.LookAt(target);
        //Debug.Log(target.position);

        //Pedestrian.SetDestination(target.position); 
        //float step = speed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        anim.SetFloat("Speed", Mathf.Clamp(accel, 0f, 2f));

        // RaycastHit hit;

        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity));


       
    }

}

