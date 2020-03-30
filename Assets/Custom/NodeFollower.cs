using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeFollower : MonoBehaviour
{
    public Node node;
    public Node nextNode;
    public int posI;
    public Vector3 targetPos;
    public float dist2node;
    public float speed;
    public float safeDist;
    //public float steering;

    public float stopDist;
    // Start is called before the first frame update
    void Awake()
    {
        //Destroy(transform.GetComponent<Collider>());

    }
    public void Setup(Node nd, float spd, float stpDist, float sfDist) {
        node = nd;
        node.occupied++;
        speed = spd;
        stopDist = stpDist;
        safeDist = sfDist;
        if (node.exits.Count != 0)
        {
            nextNode = node.exits[UnityEngine.Random.Range(0, node.exits.Count)];
        }
        else {
            nextNode = null;
        }
    }
    public void GOorWAIT()
    {
        foreach(Node n in node.conflicts)
        {
            /* If the occupied road is stopped than we can still go */
            if(n.occupied>0)
            {
                node.stop = true;
            }
            /* If the road that is one of our conflicts is not stopped and it is occupied than we should stop */
            else
            {
              /* We stop if our conflicts have cars on them */
              /* We break so that it is not reset by another node in conflicts that has no occupants */
              return;
            }
        }
        /* If no nodes have conflicts than turn the node back on and exit */
        node.stop = false;
        return;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (node.stop) return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, safeDist))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            return;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * safeDist, Color.blue);
        }

        targetPos = node.roadMovePositions[posI];
        dist2node = Vector3.Distance(transform.position, targetPos);
        //Quaternion lookAt = Quaternion.FromToRotation(transform.forward, (nodeT.position - transform.position));

        if (dist2node > stopDist)
        {
            //  Quaternion.RotateTowards(transform.rotation, lookAt, steering);
            transform.LookAt(targetPos);
            transform.position += transform.forward* speed;

        }
        else if (dist2node <= stopDist)
        {
            if (posI < node.roadMovePositions.Count - 1) posI++;
            if (posI == node.roadMovePositions.Count - 1 &&  node.exitOn)
            {
                if (nextNode == null) return;
                if (nextNode.stop) return;
                //GOorWAIT();
                node.occupied--;
                node = nextNode;
                posI = 0;
                node.occupied++;
                if (node.exits.Count>0) {
                    nextNode = node.exits[UnityEngine.Random.Range(0, node.exits.Count)];
                }
            }
            else return;
        }

    }
}
