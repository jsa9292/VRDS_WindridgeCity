using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X_manage : MonoBehaviour
{
    public List<Node> enters;
    public List<Node> intersection;
    public bool detect;
    public float detectDist;
    public int state;
    public List<bool> stateGroup;
    // Start is called before the first frame update
    void Start()
    {
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectDist);
        if (detect) {
            Collider[] c;
            enters.Clear();
            intersection.Clear();
            int lm = 1 << 9;
            c = Physics.OverlapSphere(transform.GetChild(transform.childCount - 1).position, detectDist,lm);
            foreach (Collider i in c)
            {
                Node n;
                bool cond1 = i.transform.parent.name[0] != 'X';
                bool cond2 = i.transform.GetSiblingIndex() != 0;
                if ((n = i.transform.parent.GetComponent<Node>()) && cond2 && cond1) enters.Add(n);
            }
            foreach (Node n in transform.GetComponentsInChildren<Node>()) intersection.Add(n);
            detect = false;
        }
        
    }
    void FixedUpdate() {
        switch(state){
            case 1:
                for (int i = 0; i < enters.Count; i++)
                {
                    enters[i].exitOn = stateGroup[i];

                }
                break;
            case 2:
                for (int i = 0; i < enters.Count; i++)
                {
                    enters[i].exitOn = !stateGroup[i];

                }
                break;
            case 3:
                for (int i = 0; i < enters.Count; i++)
                {
                    enters[i].exitOn = true;

                }
                break;
            default:
                for (int i = 0; i < enters.Count; i++)
                {
                    enters[i].exitOn = false;

                }
                break;
        }
        foreach (Node n in enters)
        {
            foreach (Node m in n.exits)
            {
                m.stop = !n.exitOn;
            }
        }
        foreach (Node n in enters) {
            //n.exitOn = false;
        }
        foreach (Node n in intersection) { 
            if (n.occupied>0 && !n.stop) {
                foreach (Node m in n.conflicts) {
                    m.stop = true;
                }
            }
        }
    }
}
