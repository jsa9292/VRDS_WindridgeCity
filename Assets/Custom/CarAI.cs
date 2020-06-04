using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour
{
    public NodeFollower nf; //nodefollower its following
    private Transform nfT; //transform object of the nodefollower
    public float speed; //speed of following nodefollower
    public float steering; //rot rate of the car graphics
    public float stopDist; //distance from node follower to converge. will accelerate/decelearate to this position. will back off if too close.
    //private Vector3 lookingAt; //vector dedicated for quaternion rotation. 
    public Vector3 Offset;//offsetfrom the nfT;
    public float wheelConst; //wheel graphics rot rate 
    public Transform[] wheelGraphics; // wheel graphics objects
    private Vector3 targetPos; //final position for car to follow;
    private float distance; // distance to targetPos
    private Vector3 posNoise; // noise to targetPos
    public float NoiseLevel; // noise magnitude;
    // Start is called before the first frame update
    void Start()
    {
        nfT = nf.transform;
        posNoise = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        //rb = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPos = nfT.position + Offset + posNoise* NoiseLevel;
        distance = Vector3.Distance(transform.position, targetPos);
        //lookingAt = Vector3.RotateTowards(transform.forward, nfT.forward, steering, 0.0f);
        //transform.rotation = Quaternion.LookRotation(lookingAt);
        transform.LookAt(targetPos);
        float speedFinal = (distance - stopDist) * speed;
        transform.position += transform.forward * speedFinal;//Vector3.MoveTowards(transform.position, nfT.position, (distance - stopDist) * speed);// nfT.position + transform.forward * posOffSet.x + transform.up*posOffSet.y;
        foreach (Transform t in wheelGraphics)
        {

            t.localEulerAngles += Vector3.right * distance * wheelConst;
        }
    }
}
