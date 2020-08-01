using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Animations;
using System.Linq;

public class NodeFollower : MonoBehaviour
{
    public Node node;
    public Node nextNode;
    public GameObject followedBy;
    public int posI;
    public int maxRPI;
    public Vector3 targetPos;
    public Vector3 targetDir;
    public float dist2node;
    public float speed;
	public float minspeed;
    public float maxspeed;
    public float safeDist;
    public float waitTime;
    public bool waiting = false;
    private float waitStart = -1f;
    public float steering;
    public float stopDist;
    public float movement;
    private Vector3 prev_pos;
    public bool signalStop;
    public bool signalLeft;
    public bool signalRight;
	private Camera mainCam;
	public int randseed = 0;
	public System.Random rand;
    // Start is called before the first frame update
    void Awake()
    {
        //Destroy(transform.GetComponent<Collider>());
		mainCam = Camera.main;
		rand = new System.Random(randseed);
//		int count = 0;
//		while(count <1000){
//			turns[count] = rand.Next(0,2);
//			count++;
//		}
    }
    public void Setup(Node nd) {
        node = nd;
        node.occupied++;
        if (node.exits.Count != 0)
        {
            nextNode = node.exits[rand.Next(0, node.exits.Count)];
        }
        else
        {
            nextNode = null;
        }
        posI = 0;
        maxRPI = node.roadMovePositions.Count;
        transform.position = node.roadMovePositions[0];
        targetPos = node.roadMovePositions[1];
        transform.LookAt(targetPos);
        targetDir = targetPos - transform.position;
        dist2node = targetDir.magnitude;
        //Debug.Log(targetDir);
    }
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
    // Update is called once per frame
    public void Update()
    {
        if (followedBy == null)
        {
            return;
        }
        signalLeft = false;
        signalRight = false;
        signalStop = false;
		if(node == null) return;
		if ((node.stop && posI<maxRPI/2) || waiting)
        {
            movement = 0f;
            signalStop = true;
            return;
        }

        RaycastHit hit;
        int lm;
        lm = 1 << 8;
		lm = ~lm;
		if (mainCam != null && (mainCam.transform.position - transform.position).magnitude < 5f ){
			float radius = 1f;
			if (Physics.SphereCast(transform.position,radius, transform.forward, out hit, safeDist-radius,lm))
		    {
		        Debug.DrawLine(transform.position, hit.point, Color.red);
				//StartCoroutine(Wait(waitTime));
				signalStop = true;
		        return;
		    }
		    else
		    {
		        Debug.DrawRay(transform.position, transform.forward * safeDist, Color.blue);
		    }
		}else{
			if (Physics.Raycast(transform.position,transform.forward, out hit, safeDist,lm))
			{
				Debug.DrawLine(transform.position, hit.point, Color.red);
				StartCoroutine(Wait(waitTime));
				signalStop = true;
				return;
			}
			else
			{
				Debug.DrawRay(transform.position, transform.forward * safeDist, Color.blue);
			}

		}
        //get the distance
        targetPos = followedBy.transform.position;
        targetDir = targetPos - transform.position;
        dist2node = targetDir.magnitude; 

  //      if (dist2node > stopDist)
  //      {
  //          //rotate
  //          float d_angle = Vector3.Angle(transform.forward, targetDir); // gets the difference angle
		//	// limits rotation
  //          //if (d_angle>0.01f) targetDir = Vector3.RotateTowards(transform.forward, targetPos-transform.position, d_angle*steering*Time.smoothDeltaTime, 0.0f);
  //          //transform.rotation = Quaternion.LookRotation(targetDir);
  //          //move forth
  //          speed = Mathf.Ceil((maxRPI - posI) / 10f)+minspeed; 
  //          transform.position = Vector3.MoveTowards(transform.position, targetPos, Mathf.Clamp(speed*Time.smoothDeltaTime,0,maxspeed));
            
  //      movement = Vector3.Distance(transform.position, prev_pos);
  //      prev_pos = transform.position;
		//}
        //get the next node
        if (dist2node <= stopDist)
        {
            if (posI < maxRPI - 1) posI++;
            if (posI >= maxRPI - 1)
            {
                if (nextNode == null)
                {
                    signalStop = true;
                    return;
                }
                if (!node.exitOn)
                {
                    signalStop = true;
                    return;
                }
                node.occupied--;
                node = nextNode;
                posI = 1;
                maxRPI = node.roadMovePositions.Count;
                node.occupied++;
                if (node.exits.Count > 0)
                {
					nextNode = node.exits[rand.Next(0, node.exits.Count)];
                }
				else nextNode = node;


            }
			transform.position = node.roadMovePositions[posI];
			transform.LookAt(node.roadMovePositions[posI+1]);

        }
		if(node.isIntersectionNode){
			
			if (nextNode == null) return;
			signalLeft = nextNode.leftTurn;
			signalRight = nextNode.rightTurn;
		}else{
			signalLeft = node.leftTurn;
			signalRight = node.rightTurn;
		}

    }
    IEnumerator Wait(float seconds){
		waiting = true;
		yield return new WaitForSeconds(seconds);
		waiting = false;
	}
}
