using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node : MonoBehaviour
{
    public bool loop;
    public float radius;
    public Color color1;
    public Color color2;
    public Color color3;
    public List<Node> exits;
    public List<Node> conflicts;
    public bool exitOn = true;

    public bool detectNodes;
    public bool detectCars;
    public float detectDist = 1f;
    public bool EqHeight = true;
    public float drawSensitivity;
    public int occupied;
    public bool stoping;
    public bool stop;
    public bool cleanLists;
    public bool isIntersectionNode = false;

    //Curvature Production
    public List<Vector3> roadMovePositions; //A node follower will access the nodes move positions, the cubes will be registerd into this position list also
    public bool createCurves; //check in order to add better curvature to a road
    public bool destroyCurves; //Dev tool to reset messed up curves
    public bool showCurves = false;
    public bool reverse;
    public float numSubNodes; //Determines how many sub nodes are added in between three cube nodes
    public float minDist;
    //MAX_dist - variable for later
    public int smoothCount;
    public void Awake() {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).GetComponent<Collider>());
            transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
        }
    }
    void CreateCurves()
    {
        roadMovePositions.Clear();
        int numIterations = transform.childCount;
        if (numIterations < 2) {
            return;
        }
        //Get access to first three nodes
        Transform nodeA = transform.GetChild(0);
        Transform nodeB = transform.GetChild(1);

        roadMovePositions.Add(nodeA.position);
        if (loop)
        {
            numIterations++;
        }
        for (int i = 0; i < numIterations - 1; i++)
        {
            Vector3 oldPos;
            Vector3 newPos;
            oldPos = nodeA.position;
            for (float j = 0; j <= 1f; j += 1 / numSubNodes)
            {
                newPos = nodeA.position * (1f - j) + nodeB.position * j;
                if ((oldPos - newPos).magnitude > minDist)
                {
                    roadMovePositions.Add(newPos);
                    oldPos = newPos;
                }
            }
            nodeA = NextNode(nodeA);
            nodeB = NextNode(nodeB);

        }
        //adding subnodes

        //smoothing
        for (int j = 0; j < smoothCount; j++)
        {
            for (int i = 1; i < roadMovePositions.Count - 1; i++)
            {
                roadMovePositions[i] = roadMovePositions[i - 1] * .3f + roadMovePositions[i] * .4f + roadMovePositions[i + 1] * .3f;
            }
        }
        roadMovePositions.Add(nodeB.position);

    }
    void OnDrawGizmosSelected() {
        Gizmos.color = color1;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentNode = transform.GetChild(i);
            Gizmos.DrawLine(currentNode.position, currentNode.position + currentNode.up);
            Gizmos.DrawLine(currentNode.position, NextNode(currentNode).position);
        }
        if (reverse)
        {
            roadMovePositions.Reverse();
            reverse = false;
        }
        if (destroyCurves)
        {
            roadMovePositions.Clear();
            destroyCurves = false;
        }
        if (createCurves)
        {
            createCurves = false;
            CreateCurves();
        }
        if (detectCars)
        {
            Collider[] c;
            occupied = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                c = Physics.OverlapSphere(transform.GetChild(i).position, detectDist);
                NodeFollower nf;
                foreach (Collider j in c)
                {
                    if (nf = j.transform.GetComponent<NodeFollower>())
                    {
                        nf.node = this;
                        occupied++;
                    }
                }
            }
            detectCars = false;
        }
        if (detectNodes)
        {
            Collider[] c;
            exits.Clear();
            int lm = 1 << 9;
            c = Physics.OverlapSphere(transform.GetChild(transform.childCount - 1).position, detectDist,lm);
            foreach (Collider i in c)
            {
                Node n;
                if ((n = i.transform.parent.GetComponent<Node>()) && (i.transform.parent != transform))
                {
                    i.transform.position = transform.GetChild(transform.childCount - 1).position;
                    if(i.transform.parent.name[0] != 'X')exits.Add(n);
                }
            }
            detectNodes = false;

        }
        if (EqHeight)
        {
            int lm = 1 << 9;
            lm = ~lm;

            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 p = transform.GetChild(i).position;
                RaycastHit hit;
                if (Physics.Raycast(transform.GetChild(i).position + Vector3.up * 10000f, -Vector3.up, out hit,20000f,lm))
                {
                    transform.GetChild(i).position = hit.point + Vector3.up * 1f;
                    //Debug.DrawLine(transform.GetChild(i).position + Vector3.up * 10000f,hit.point);
                }
            }
            EqHeight = false;
        }
        if (cleanLists) {
            exits.RemoveAll(node => node == null);
            conflicts.RemoveAll(node => node == null);
            cleanLists = false;
        }
    }
    void OnDrawGizmos()    {

        /* Makes it so that we can see which paths are turned off */
        if (stop) return;

        //Function to turn the move positions on or off
        if (showCurves && roadMovePositions != null)
        {
            if (occupied == 0) Gizmos.color = color2;
            else Gizmos.color = color3;
          for(int i = 0; i<roadMovePositions.Count-1; i++)
           {
                Gizmos.DrawLine(roadMovePositions[i], roadMovePositions[i + 1]);
                Gizmos.DrawLine(roadMovePositions[i+1], roadMovePositions[i+1]+ Vector3.up*.5f +Vector3.up*1.5f*i/ roadMovePositions.Count);
           }
        }


    }
    public Transform NextNode(Transform nodeTnow)
    {
        if (stop) return nodeTnow;
        if (nodeTnow.transform.parent != transform) return transform.GetChild(0);
        if (nodeTnow.GetSiblingIndex() + 1 < transform.childCount)
            return nodeTnow.parent.GetChild(nodeTnow.GetSiblingIndex() + 1);
        else if (loop)
            return nodeTnow.parent.GetChild(0);
        else return nodeTnow;
    }
}
