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
	public int stopBefore;
    public Vector3 targetPos;
    public Vector3 targetDir;
    public float dist2node;
	public float dist2car;
    public float safeDist;
    public float waitTime;
    public bool waiting = false;
    private float waitStart = -1f;
    public float steering;
    public float stopDist;
    public bool signalStop;
    public bool signalLeft;
    public bool signalRight;
	private Camera mainCam;
	public int randseed = 0;
	public System.Random rand;
	public LayerMask lm;
	public List<Vector3> targetPoses;
	public int memory;
    // Start is called before the first frame update
    void Awake()
    {
        //Destroy(transform.GetComponent<Collider>());
		mainCam = Camera.main;
		rand = new System.Random(randseed);
		targetPoses = new List<Vector3>();
		targetPoses.Add(transform.position);
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
		Gizmos.color = signalStop ? Color.red:Color.white;
        Gizmos.DrawWireSphere(transform.position, safeDist);
    }
    // Update is called once per frame
	private RaycastHit hit;
	public bool raycast;
    public void Update()
    {
		//Debug.Log("Nf update");
        if (followedBy == null)
        {
			Debug.Log("FollowedBy is Null");
            return;
        }
        signalLeft = false;
        signalRight = false;
		signalStop = false;
		if (nextNode == null) return;
		signalLeft = node.leftTurn ||(nextNode.leftTurn&&((maxRPI-posI)<10));
		signalRight = node.rightTurn||(nextNode.rightTurn&&((maxRPI-posI)<10));
		if(node == null) {
			Debug.Log("Node is Null");
			return;
		}
		if ((node.stop && posI<maxRPI/2) || waiting)
        {
            signalStop = true;
        }

		Debug.DrawRay(targetPos,Vector3.up*2f,Color.white);
		if(raycast){
			Collider[] hitCols = Physics.OverlapSphere(transform.position,safeDist,lm);
			if (hitCols.Length >0)
			{
				foreach (Collider c in hitCols){
					if(c.transform.parent.parent != followedBy.transform){
						signalStop = true;
						Wait(waitTime);
						return;
					}
				}
			}
//			if (Physics.Raycast(transform.position+transform.up*.5f,transform.forward, out hit, safeDist,lm))
//			{
//				Debug.DrawLine(transform.position+transform.up*.5f, hit.point, Color.red);
//				//StartCoroutine(Wait(waitTime));
//				signalStop = true;
//				if((transform.position - hit.point).magnitude < stopDist) return;
//			}
//			else
//			{
//				Debug.DrawRay(transform.position+transform.up*.5f, transform.forward * safeDist, Color.blue);
//			}
		}
		if(posI>maxRPI-stopBefore && !node.exitOn) signalStop = true;
        //get the distance
        targetPos = targetPoses[0];
		targetDir =  targetPos - followedBy.transform.position;
        dist2node = targetDir.magnitude; 
		//Debug.Log("targetPosed");
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
		dist2car = (transform.position - followedBy.transform.position).magnitude;
		if (dist2car <= stopDist)
        {
			if (posI < maxRPI - 1) {
				posI++;
			}
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
			targetPoses.Add(transform.position);
			if(targetPoses.Count>memory)targetPoses.RemoveAt(0);
			transform.position = node.roadMovePositions[posI];
			transform.LookAt(node.roadMovePositions[posI+1]);

		}
//		if(node.isIntersectionNode){
//			
//			if (nextNode == null) return;
//			signalLeft = nextNode.leftTurn;
//			signalRight = nextNode.rightTurn;
//		}else{
//			signalLeft = node.leftTurn;
//			signalRight = node.rightTurn;
//		}

    }
    IEnumerator Wait(float seconds){
		waiting = true;
		yield return new WaitForSeconds(seconds);
		waiting = false;
	}
}
