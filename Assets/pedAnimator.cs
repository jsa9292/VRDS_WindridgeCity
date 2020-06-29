using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pedAnimator : MonoBehaviour
{
    public Animator anim;
    public float offset;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetFloat("offset", offset);
    }
}
