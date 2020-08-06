using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class pedAI : MonoBehaviour
{
    public NodeFollower nf;
    private Transform nfT;
    private Animator anim;
    public float speed;
    public float speedCoeff;
	public bool onPhone;
    // Update is called once per frame
    private void Start()
    {
        if (nf != null) nfT = nf.transform;
        anim = transform.GetComponent<Animator>();
        anim.SetFloat("Speed", 0);
		if(onPhone){
			anim.SetBool("OnPhone",onPhone);
		}
    }
	private float dirDiff;
	private Vector3 lookingAt;
    void LateUpdate()
    {
        if (nf == null) return;
		if (nf.followedBy == null) nf.followedBy = transform.gameObject;
        if (nf.waiting) {
            anim.SetBool("Waiting", true);
            return;
        }
		dirDiff = Vector3.Dot(transform.forward,(nf.transform.position-transform.position).normalized);
		lookingAt = Vector3.RotateTowards(transform.forward, (nf.transform.position-transform.position), (1f-dirDiff), 0.0f);//nfT.forward
		transform.rotation = Quaternion.LookRotation(lookingAt);
		 anim.SetFloat("Speed", speed * speedCoeff);
        if (speed == 0f) return;
		transform.position += transform.forward * speed* Time.smoothDeltaTime ;

    }
}
