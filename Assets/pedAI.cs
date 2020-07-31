using AmplifyShaderEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pedAI : MonoBehaviour
{
    public NodeFollower nf;
    private Transform nfT;
    private Animator anim;
    public float speed;
    public float speedCoeff;
    public float catchup;
    // Update is called once per frame
    private void Start()
    {
        if (nf != null) nfT = nf.transform;
        anim = transform.GetComponent<Animator>();
        anim.SetFloat("Speed", 0);
    }
    void LateUpdate()
    {
        if (nf == null) return;
        if (nf.waiting) {
            anim.SetBool("Waiting", true);
            return;
        }
        transform.LookAt(nf.transform);
        speed = (transform.position - nfT.position).magnitude * Time.smoothDeltaTime;
        anim.SetFloat("Speed", speed * speedCoeff);
        if (speed == 0f) return;
        transform.position += transform.forward * speed * catchup ;

    }
}
