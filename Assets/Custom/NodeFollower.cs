using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeFollower : MonoBehaviour
{
    public Node node;
    public Transform nodeT;
    public float speed;
    public float safeDist;
    //public float steering;

    public float stopDist;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (nodeT == null) return;
        int layerMask = 1 << 5;
        layerMask = ~layerMask;

        transform.LookAt(nodeT);
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

        float dist2node = Vector3.Distance(transform.position, nodeT.position);
        //Quaternion lookAt = Quaternion.FromToRotation(transform.forward, (nodeT.position - transform.position));
        
        if (dist2node > stopDist)
        {
            //  Quaternion.RotateTowards(transform.rotation, lookAt, steering);
            transform.position += transform.forward* speed;
        }else if (dist2node <= stopDist)
        {
            if (node.exitOn && nodeT.GetSiblingIndex() == nodeT.transform.parent.childCount - 1 && node.exits.Count !=0) node = node.exits[Random.Range(0, node.exits.Count)];
            nodeT = node.NextNode(nodeT);
        }
        
    }
}
