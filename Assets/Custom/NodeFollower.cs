using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeFollower : MonoBehaviour
{
    public Node node;
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

    public void GOorWAIT()
    {
      if (node.isIntersectionNode)
      {
        foreach(Node n in node.conflicts)
        {
          if(n.occupied > 0)
          {
            /* If the occupied road is stopped than we can still go */
            if(n.stop)
            {
              continue;
            }
            /* If the road that is one of our conflicts is not stopped and it is occupied than we should stop */
            else
            {
              /* We stop if our conflicts have cars on them */
              node.stop = true;
              /* We break so that it is not reset by another node in conflicts that has no occupants */
              return;
            }
          }
        }
        /* If no nodes have conflicts than turn the node back on and exit */
        node.stop = false;
        return;
      }
      /* If it is just a normal road node than we do not need to do anything */
      else
      {
        return;
      }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(node.stop)
        {
          GOorWAIT();
        }
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
            if (posI == node.roadMovePositions.Count - 1 && node.exits.Count != 0)
            {
                node.occupied--;
                node = node.exits[UnityEngine.Random.Range(0, node.exits.Count)];
                posI = 0;
                node.occupied++;
                GOorWAIT();
            }
            else if (node.exits.Count == 0) return;
        }

    }
}
