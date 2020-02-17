using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X_manage : MonoBehaviour
{
    public bool detect;
    public List<Transform> enters;
    public List<Transform> exits;
    // Start is called before the first frame update
    void Start()
    {

    }
    private float count;
    private Collider[] near;
    public float nearDist;
    void OnDrawGizmosSelected()
    {
        count += Time.deltaTime;
        Gizmos.DrawWireSphere(transform.position, nearDist);
        if (count>30f && detect)
        {
            enters.Clear();
            exits.Clear();
            near = Physics.OverlapSphere(transform.position, nearDist);
            foreach (Collider c in near) {
                if (c.transform.GetSiblingIndex() == 0) enters.Add(c.transform);
                else exits.Add(c.transform); 
            }
            Debug.Log("getting nearby nodes...");
            detect = false;
        }
        if (enters.Count == exits.Count) {
            for (int i = 0; i < enters.Count; i++) {
                Gizmos.DrawLine(enters[i].position, exits[i].position);
            }
        }
    }
    public Transform enter2exit(Transform enterFrom) {
        int enteri = enters.FindIndex(t =>t==enterFrom);
        List<Transform> temp = exits;
        temp.RemoveAt(enteri);
        return temp[Random.Range(0, temp.Count)];
    }
    private bool isMatch(Transform t1, Transform t2) {
        return t1 == t2; 
    }

}
