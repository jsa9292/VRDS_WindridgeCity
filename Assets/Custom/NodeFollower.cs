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
        Destroy(transform.GetComponent<Collider>());   
    }
    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, detectDist);
    //    if (detect)
    //    {
    //        Collider[] c;
    //        c = Physics.OverlapSphere(transform.position, detectDist);
    //        foreach (Collider i in c)
    //        {
    //            Node n;
    //            try
    //            {
    //                if (n = i.transform.parent.GetComponent<Node>())
    //                {
    //                    node = n;
    //                    nodeT = i.transform;
    //                    node.occupied++;
    //                }
    //            }
    //            catch (Exception e) { }
    //        }
    //        detect = false;
    //    }
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (node.stop) return;

        int layerMask = 1 << 5;
        layerMask = ~layerMask;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, safeDist, layerMask))
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
                if (node.occupied == 0)
                {
                    foreach (Node n in node.conflicts)
                    {
                        n.stop = false;
                    }
                }
                node = node.exits[UnityEngine.Random.Range(0, node.exits.Count)];
                posI = 0;
                node.occupied++;
            }
            else if (node.exits.Count == 0) return;
        }
        
    }
}
