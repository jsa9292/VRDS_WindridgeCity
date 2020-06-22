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
    public Vector3 targetPos;
    public Vector3 targetDir;
    public float dist2node;
    public float speed;
    public float safeDist;
    public float waitTime;
    public bool waiting = false;
    private float waitStart = -1f;
    private float waited;
    public float steering;
    public float stopDist;
    public float movement;
    private Vector3 prev_pos;
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
        transform.position = node.roadMovePositions[0];
        targetPos = node.roadMovePositions[1];
        transform.LookAt(targetPos);
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
        if (node.stop)
        {
            movement = 0f;
            return;
        }

        int lm = 0 << 5;
        lm = ~lm;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, safeDist,lm))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            return;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * safeDist, Color.blue);
        }

        //get the distance
        dist2node = Vector3.Distance(transform.position, targetPos);
        if (dist2node > stopDist)
        {
            //rotate
            //float d_angle = Vector3.Angle(transform.forward, targetPos - transform.position);
            //if (d_angle>0.01f) targetDir = Vector3.RotateTowards(transform.forward, targetPos-transform.position, d_angle*steering, 0.0f);
            //transform.rotation = Quaternion.LookRotation(targetDir);
            //move forth
            movement = Vector3.Distance(transform.position, prev_pos);
            prev_pos = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);

        }
        //get the next node
        else if (dist2node <= stopDist)
        {
            if (posI < node.roadMovePositions.Count - 1) posI++;
            if (posI == node.roadMovePositions.Count - 1)
            {
                if (nextNode == null) return;
                if (!node.exitOn) return;
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
                    return;
                }
                waiting = false;
                node.occupied--;
                node = nextNode;
                posI = 0;
                node.occupied++;
                if (node.exits.Count > 0)
                {
                    nextNode = node.exits[UnityEngine.Random.Range(0, node.exits.Count)];
                }

            }
            targetPos = node.roadMovePositions[posI+1];
            //targetDir = targetPos - transform.position;
            transform.LookAt(targetPos);
        }

    }
}
