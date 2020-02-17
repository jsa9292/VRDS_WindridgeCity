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

    public WheelCollider[] wheels;
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
        Quaternion lookAt = Quaternion.FromToRotation(transform.forward, (nfT.position - transform.position));
        float ang2nf = Vector3.SignedAngle(transform.forward, nf.transform.position-transform.position, transform.up);

        if (dist2nf > stopDist)
        {
            //Quaternion.RotateTowards(transform.rotation, lookAt, steering);
            //transform.LookAt(nfT);
            //transform.position += transform.forward * dist2nf*speed;
            float steer = Mathf.Clamp(ang2nf, -steering, steering);
            float motor = Mathf.Clamp((dist2nf - stayDist)/stayDist,0,1);
            rb.drag = -Mathf.Tan(Mathf.Clamp(dist2nf,0,Mathf.PI/2)+ Mathf.PI / 4);
            wheels[0].steerAngle = steer;
            wheels[2].motorTorque = Mathf.Pow(motor,3)*speed;

            wheels[1].steerAngle = wheels[0].steerAngle;
            wheels[3].motorTorque = wheels[2].motorTorque;
            //Debug.Log(dist2nf);
            wheels[0].brakeTorque = 0;
            wheels[1].brakeTorque = 0;
            Vector3 pos;
            Quaternion quat;
            for (int i = 0; i < 4; i++)
            {

                wheels[i].GetWorldPose(out pos, out quat);
                wheelGraphics[i].SetPositionAndRotation(pos, quat);
            }
        }
        else if (dist2nf <= stopDist) {
            wheels[0].brakeTorque = brakePower;
            wheels[1].brakeTorque = brakePower;
        }
     }
}
