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
        foreach (Node n in intersection) {
            n.isIntersectionNode = true;
        }
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


    private int xI;
    void FixedUpdate()
    {
        switch (state)
        {
            /* Vertical or horizontal */
            case 1:
                for (int i = 0; i < enters.Count; i++)
                {
                    enters[i].exitOn = stateGroup[i];

                }
                break;
            /* Vertical or horizontal */
            case 2:
                for (int i = 0; i < enters.Count; i++)
                {
                    enters[i].exitOn = !stateGroup[i];
                }
                break;
            /* All directions are open, 4 way stop, whichever car gets there first should get there first */
            case 3:
                for (int i = 0; i < enters.Count; i++)
                {
                    enters[i].exitOn = false;
                }
                break;
            /* Closes all ways */
            default:
                return;
        }
        // This enforces nodefollowers to waitfor execution of rest of X_manage script before allowed to switch to intersection node
        // The problem was 1, nodefollowers switched node before checking if it was available,
        // then the multiple instances of nodefollowers switched to intersection node before conflicts were updated.
        // instead of updating conflict for each switch which increases operations count,
        // shutting down allows reducing the operation count
        enters[xI].exitOn = true;
        xI++;
        if (xI >= enters.Count) xI = 0;

        //for each intersection, if any of the conflict is occupied, it is disabled. 
        foreach (Node n in intersection) {
            n.stop = false;
            foreach (Node m in n.conflicts) {
                n.stop = n.stop || (m.occupied > 0);
            }
        }
    }
}
