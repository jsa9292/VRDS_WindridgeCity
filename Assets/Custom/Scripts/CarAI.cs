using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VLB;

public class CarAI : MonoBehaviour
{
    public NodeFollower nf; //nodefollower its following
    private Transform nfT; //transform object of the nodefollower
    public float speed; //speed of following nodefollower
    public float steering; //rot rate of the car graphics
    public float stopDist; //distance from node follower to converge. will accelerate/decelearate to this position. will back off if too close.
    private Vector3 lookingAt; //vector dedicated for quaternion rotation. 
    public Vector3 Offset;//offsetfrom the nfT;
    public float wheelConst; //wheel graphics rot rate 
    public Transform[] wheelGraphics; // wheel graphics objects
    public Transform[] wheelParents;
	public Rigidbody rb;
	public float dirDiff;
	public float dirWeight;
	[Range(0.0f,5.0f)]
	public float speedAutoCorrCoeff;
	//Graphics
    public Renderer carBody;
    public Renderer tailLight;
    public VolumetricLightBeam Lsignal;
    public VolumetricLightBeam Rsignal;
    private MaterialPropertyBlock mpb;
	public float speedFinal;

	public bool debug;
	private float speed_prev;
	private float Force;
	private float Momentum;
    // Update is called once per frame
    void Update()
    {
        
        if (nf != null)
        {
            nfT = nf.transform;
            nf.followedBy = gameObject;
        }
        if (rb == null) rb = transform.GetComponent<Rigidbody>();
        if (!rb.isKinematic) return;

        //while ((nf.transform.position - transform.position).magnitude<=stopDist) {
        //    nf.UpdateNF(); 
        //}
		dirDiff = Vector3.Dot(transform.forward,nf.targetDir.normalized);
		if(debug)Debug.Log(dirDiff);
		dirDiff = Mathf.Pow(dirDiff,dirWeight);
		Force = nf.signalStop ? 0:1 * speed *(Time.deltaTime*speedAutoCorrCoeff)* dirDiff;
		Momentum = speed_prev *(1f-Time.deltaTime*speedAutoCorrCoeff);
		speedFinal =  Force  + Momentum;
		speed_prev = speedFinal;
		//if(debug)Debug.Log(Force + " + " + Momentum + " = " + speedFinal);
		lookingAt = Vector3.RotateTowards(transform.forward, nf.targetDir, steering *(1f-dirDiff), 0.0f);//nfT.forward
		transform.rotation = Quaternion.LookRotation(lookingAt);
		if((nf.targetPos-transform.position).magnitude >.1f){
			if(nf.signalStop && nf.targetPoses.Count>1)nf.targetPoses.RemoveAt(0);
		}else return;
		transform.position += transform.forward * speedFinal* Time.smoothDeltaTime;

		if (speedFinal > 0f)
        {
			//Vector3.MoveTowards(transform.position, nfT.position, (distance - stopDist) * speed);// nfT.position + transform.forward * posOffSet.x + transform.up*posOffSet.y;
            //targetDir
            //transform.LookAt(targetPos);

        }
        //cuts update for visual if the LOD0 body is not visible
        if (carBody != null && !carBody.isVisible) return;

        foreach (Transform t in wheelParents)
        {

            t.LookAt(t.position + nf.targetDir);
        }
        foreach (Transform t in wheelGraphics)
        {

            t.localRotation *= Quaternion.Euler(speedFinal * wheelConst*Time.smoothDeltaTime, 0f, 0f);

        }
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
            tailLight.GetPropertyBlock(mpb);
            mpb.SetFloat("_LR", 1f);
            mpb.SetFloat("_BrakeOnOff", 1f);
            mpb.SetFloat("_BothOn", 1f);
        }
        mpb.SetFloat("_BothOn", 0f);
        mpb.SetFloat("_LR", 0f);
        Lsignal.intensityGlobal = 0f;
        Rsignal.intensityGlobal = 0f;
        float flickerStrength = (Mathf.Sin(Time.time) + 1) * 8f;
        if (nf.signalRight) 
        {
            mpb.SetFloat("_LR", 1f);
            Rsignal.intensityGlobal = flickerStrength;
        }
        if (nf.signalLeft)
        {
            mpb.SetFloat("_LR", -1f);
            Lsignal.intensityGlobal = flickerStrength;
        }
        mpb.SetFloat("_BrakeOnOff", nf.signalStop ? 1f:0f);
        tailLight.SetPropertyBlock(mpb);

    }
//    private bool makeWheelParents = false;
//    //This was meant to format and setup the vehicle prefab because all four wheels were under "Prefab"
//    //The front two wheels need to have parents for y axis rotation, hence the function creates and setup the rest of the scripts.
//    //must be ran in prefab stage as the editor window will not allow prefab will change in scene window.
//    //make it public to use it;
//    void MakeWheelParent()
//    {
//        for (int i = 0; i < 2; i++)
//        {
//            GameObject gn = new GameObject("wheelParent");
//            GameObject g = GameObject.Instantiate(gn,wheelGraphics[i].parent);
//            g.transform.position = wheelGraphics[i].position;
//            wheelGraphics[i].parent = g.transform;
//            wheelParents[i] = g.transform;
//        }
//
//    }
//    private void OnDrawGizmosSelected()
//    {
//        if (makeWheelParents)
//        {
//            makeWheelParents = false;
//            MakeWheelParent();
//        }
//    }
//	void OnCollisionEnter(Collision c){
//		rb.isKinematic = false;
//		rb.useGravity = true;
//	}
}
