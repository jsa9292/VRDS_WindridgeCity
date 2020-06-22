using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorCamera : MonoBehaviour
{
	public Camera reflect;
	public Transform eyes;
	public Vector3 result;
	public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
	{

		//angle = eyes.position- reflect.transform.position;
		Vector3 reflectedPos = Vector3.Reflect(eyes.position,transform.up);
		Vector3 reflectedDir = reflectedPos-transform.position;
		reflect.transform.rotation = Quaternion.LookRotation(-reflectedDir,Vector3.up);
		reflect.transform.localEulerAngles += offset;
    }
}
