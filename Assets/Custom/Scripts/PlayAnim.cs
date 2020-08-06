using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnim : MonoBehaviour
{
	public Animation anim;
    // Start is called before the first frame update
    void Start()
    {
		anim.Play("New Animation");
    }

}
