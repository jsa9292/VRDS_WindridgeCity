using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltPassing : MonoBehaviour
{

    public Collider TiltZone;
    public Transform spine;
    public float maxTilt;
    public float tiltRate;
    private float tiltAccum;
    // Update is called once per frame
    void LateUpdate()
    {
        tiltAccum = Mathf.Clamp(tiltAccum, 0, 1);
        spine.eulerAngles += Vector3.up * Mathf.Lerp(0, maxTilt, tiltAccum);
    }
    void OnTriggerEnter(Collider c)
    {
        if (c == TiltZone) tiltAccum += tiltRate * Time.deltaTime;
    }
    void OnTriggerExit(Collider c) {
        if (c == TiltZone) tiltAccum -= tiltRate * Time.deltaTime;
    }
}
