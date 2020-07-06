using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Animations;

public class NodeFollower : MonoBehaviour
{
    public Node node;
    public Node nextNode;
    public int posI;
    private int maxRPI;
    public Vector3 targetPos;
    public Vector3 targetDir;
    public float dist2node;
    public float speed;
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
    // Start is called before the first frame update
    void Awake()
    {
        //Destroy(transform.GetComponent<Collider>());

    }
    public void Setup(Node nd, float spd, float stpDist, float sfDist, float wTime) {
        node = nd;
        node.occupied++;
        speed = spd;
        stopDist = stpDist;
        safeDist = sfDist;
        waitTime = wTime;
        if (node.exits.Count != 0)
        {
            nextNode = node.exits[UnityEngine.Random.Range(0, node.exits.Count)];
        }
        else {
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
    //public void GOorWAIT()
    //{
    //    foreach(Node n in node.conflicts)
    //    {
    //        /* If the occupied road is stopped than we can still go */
    //        if(n.occupied>0)
    //        {
    //            node.stop = true;
    //        }
    //        /* If the road that is one of our conflicts is not stopped and it is occupied than we should stop */
    //        else
    //        {
    //          /* We stop if our conflicts have cars on them */
    //          /* We break so that it is not reset by another node in conflicts that has no occupants */
    //          return;
    //        }
    //    }
    //    /* If no nodes have conflicts than turn the node back on and exit */
    //    node.stop = false;
    //    return;
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        signalLeft = false;
        signalRight = false;
        signalStop = movement <0.01f;
        
        if (node.stop)
        {
            movement = 0f;
            signalStop = true;
            return;
        }

        RaycastHit hit;
        int lm;
        lm = 0 << 5;
        lm = ~lm;
        if (Physics.SphereCast(transform.position,.2f, transform.forward, out hit, safeDist,lm))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            signalStop = true;
            return;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * safeDist, Color.blue);
        }

        //get the distance
        targetDir = targetPos - transform.position;
        dist2node = targetDir.magnitude; 

        if (dist2node > stopDist)
        {
            //rotate
            float d_angle = Vector3.Angle(transform.forward, targetDir); // gets the difference angle
			// limits rotation
            if (d_angle>0.01f) targetDir = Vector3.RotateTowards(transform.forward, targetPos-transform.position, d_angle*steering, 0.0f);
            transform.rotation = Quaternion.LookRotation(targetDir);
            //move forth
             transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
            
        movement = Vector3.Distance(transform.position, prev_pos);
        prev_pos = transform.position;
        }
        //get the next node
        else if (dist2node <= stopDist)
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
                if (nextNode.stop)
                {
                    if (!waiting)
                    {
                        waitStart = Time.realtimeSinceStartup;
                        waiting = true;
                    }
                    else if (waiting && Time.realtimeSinceStartup - waitStart > waitTime)
                    {
                        waitStart = -1f;
                        nextNode = node.exits[UnityEngine.Random.Range(0, node.exits.Count)];
                        waiting = false;
                    }
                    signalStop = true;
                    return;
                }
                waiting = false;
                node.occupied--;
                node = nextNode;
                posI = 1;
                maxRPI = node.roadMovePositions.Count;
                node.occupied++;
                if (node.exits.Count > 0)
                {
                    nextNode = node.exits[UnityEngine.Random.Range(0, node.exits.Count)];
                }
                else nextNode = node;

            }
            targetPos = node.roadMovePositions[posI];
            
            //transform.LookAt(targetPos);
        }
        if (maxRPI - posI < 20)
        {
            if (nextNode == null) return;
            signalLeft = nextNode.leftTurn || node.leftTurn;
            signalRight = nextNode.rightTurn || node.rightTurn;
        }
    }
}
