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
    public Transform[] wheelParents;
	public Rigidbody rb;
    private Vector3 targetPos; //final position for car to follow;
    private float distance; // distance to targetPos
    private Vector3 posNoise; // noise to targetPos
    public float NoiseLevel; // noise magnitude;
    public GameObject stopParent;
    public GameObject leftParent;
    public GameObject rightParent;
    public Light[] stopFlares;
    public Light[] leftFlares;
    public Light[] rightFlares;
    // Start is called before the first frame update
    void Start()
    {
        nfT = nf.transform;
        posNoise = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        posNoise *= NoiseLevel;
        stopFlares = stopParent.GetComponentsInChildren<Light>();
        leftFlares = leftParent.GetComponentsInChildren<Light>();
        rightFlares = rightParent.GetComponentsInChildren<Light>();
        rb = transform.GetComponent<Rigidbody>();
    }
	private float speedFinal;
    // Update is called once per frame
    void Update()
    {
		if(!rb.isKinematic) return;
        targetPos = nfT.position + Offset + posNoise;
        distance = Vector3.Distance(transform.position, targetPos);
        //lookingAt = Vector3.RotateTowards(transform.forward, nfT.forward, steering, 0.0f);
        //transform.rotation = Quaternion.LookRotation(lookingAt);
        transform.LookAt(targetPos);
        speedFinal = (distance - stopDist) * speed* 0.5f + speedFinal*0.5f;
        transform.position += transform.forward * speedFinal;//Vector3.MoveTowards(transform.position, nfT.position, (distance - stopDist) * speed);// nfT.position + transform.forward * posOffSet.x + transform.up*posOffSet.y;
        float wheel_y;
        foreach (Transform t in wheelParents) {

            t.LookAt(t.position + nf.targetDir);
        }
        foreach (Transform t in wheelGraphics){
    
            t.localRotation *= Quaternion.Euler(speedFinal*wheelConst,0f,0f);
            
        }
        float frequency = 8f;
        float magnitude = 2f;
        foreach (Light l in leftFlares)
        {
            l.enabled = nf.signalLeft;
			if(l.enabled) l.intensity = Mathf.Sin(Time.realtimeSinceStartup* frequency) * magnitude + magnitude/2f;
        }
        foreach (Light l in rightFlares)
        {
            l.enabled = nf.signalRight;
			if(l.enabled) l.intensity = Mathf.Sin(Time.realtimeSinceStartup* frequency) * magnitude + magnitude/2f;
        }
        foreach (Light l in stopFlares)
        {
            l.enabled = nf.signalStop;
        }

    }
    public bool makeWheelParents = false;
    //This was meant to format and setup the vehicle prefab because all four wheels were under "Prefab"
    //The front two wheels need to have parents for y axis rotation, hence the function creates and setup the rest of the scripts.
    //must be ran in prefab stage as the editor window will not allow prefab will change in scene window.
    void MakeWheelParent()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject gn = new GameObject("wheelParent");
            GameObject g = GameObject.Instantiate(gn,wheelGraphics[i].parent);
            g.transform.position = wheelGraphics[i].position;
            wheelGraphics[i].parent = g.transform;
            wheelParents[i] = g.transform;
        }

    }
    private void OnDrawGizmosSelected()
    {
        if (makeWheelParents)
        {
            makeWheelParents = false;
            MakeWheelParent();
        }
    }
//	void OnCollisionEnter(Collision c){
//		rb.isKinematic = false;
//		rb.useGravity = true;
//	}
}
