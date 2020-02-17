using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool loop;
    public float radius;
    public Color color1;
    public Color color2;
    public List<Node> exits;
    public bool exitOn = true;

    public bool detect;
    public float detectDist = 1f;
    public bool merge = true;
    public bool EqHeight = true;
    public float drawSensitivity;
    public int subdiv;
    //void OnDrawGizmos() {
    //    Gizmos.color = color2;
    //    for (int i = 0; i < transform.childCount * subdiv; i++)
    //    {
    //        Transform currentNode = transform.GetChild(i / subdiv);
    //        Transform nextNode = NextNode(currentNode);
    //        float intpolW = (float)((i % subdiv) / (float)subdiv);
    //        //Debug.Log(intpolW);
    //        float intpolW_next = (float)(((i + 1) % subdiv) / (float)subdiv);
    //        Gizmos.DrawLine(currentNode.position* (1f- intpolW) + nextNode.position * intpolW, currentNode.position * (1f - intpolW) + nextNode.position * intpolW + currentNode.up);
    //        Gizmos.DrawLine(currentNode.position * (1f - intpolW) + nextNode.position * intpolW, currentNode.position * (1f - intpolW_next) + nextNode.position * intpolW_next);

    //    }
    //}
    void OnDrawGizmos()
    {
        Gizmos.color = color1;
        for (int i = 0; i < transform.childCount;  i++) {
            Transform currentNode = transform.GetChild(i);
            Gizmos.DrawLine(currentNode.position, currentNode.position+ currentNode.up);
            Gizmos.DrawLine(currentNode.position, NextNode(currentNode).position);
        }

        if (detect)
        {
            Collider[] c;
            exits.Clear();
            c = Physics.OverlapSphere(transform.GetChild(transform.childCount - 1).position, detectDist);
            foreach (Collider i in c)
            {
                Node n;
                if ((n = i.transform.parent.GetComponent<Node>())&&i.transform.GetSiblingIndex()==0&&(i.transform.parent != transform)) exits.Add(n);
            }
            detect = false;

        }
        if (merge)
        {
            foreach (Node n in exits)
            {
                transform.GetChild(transform.childCount - 1).position=n.transform.GetChild(0).position;
            }
            merge = false;
        }
        if (EqHeight) {
            for (int i = 0; i < transform.childCount; i++) {
                Vector3 p = transform.GetChild(i).position;
                RaycastHit hit;
                if (Physics.Raycast(transform.GetChild(i).position + Vector3.up*10000f, -Vector3.up, out hit))
                {
                    transform.GetChild(i).position = hit.point+Vector3.up*1f;
                    //Debug.DrawLine(transform.GetChild(i).position + Vector3.up * 10000f,hit.point);
                }
            }
            EqHeight = false;
        }
    }
    public Transform NextNode(Transform nodeTnow)
    {
        if (nodeTnow.transform.parent != transform) return transform.GetChild(0);
        if (nodeTnow.GetSiblingIndex() + 1 < transform.childCount)
            return nodeTnow.parent.GetChild(nodeTnow.GetSiblingIndex() + 1);
        else if (loop)
            return nodeTnow.parent.GetChild(0);
        else return nodeTnow;
    }
}
