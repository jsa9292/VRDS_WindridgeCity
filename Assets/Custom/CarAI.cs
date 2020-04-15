using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public NodeFollower nf;
    private Transform nfT;
    public float speed;
    public float steering;
    public float stayDist;
    public float stopDist;
    public float brakePower;
    public float recSpeed;

    public Transform[] wheelGraphics;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        nfT = nf.transform;
        rb = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (nfT == null) return;
        float dist2nf = Vector3.Distance(transform.position, nfT.position);
        //Quaternion lookAt = Quaternion.FromToRotation(transform.forward, (nfT.position - transform.position));
        //float ang2nf = Vector3.SignedAngle(nf.transform.position,transform.position);

        if (dist2nf > stopDist)
        {
            //Quaternion.RotateTowards(transform.rotation, lookAt, steering);
            //rb.MoveRotation(rb.rotation*Quaternion.Euler(ang2nf*steering));
            transform.LookAt(nfT);
            rb.MovePosition(transform.position + transform.forward * (dist2nf-stopDist)*speed);
            //float steer = Mathf.Clamp(ang2nf, -steering, steering);
            //float motor = Mathf.Clamp((dist2nf - stayDist)/stayDist,0,1);
            //rb.drag = -Mathf.Tan(Mathf.Clamp(dist2nf,0,Mathf.PI/2)+ Mathf.PI / 4);
           
            //Vector3 pos;
            //Quaternion quat;
            //for (int i = 0; i < 4; i++)
            //{
                //wheelGraphics[i].SetPositionAndRotation(pos, quat);
            //}
        }
        else if (dist2nf <= stopDist) {
        }
     }
}
