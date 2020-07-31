using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour
{
    public float height;
    public float speed;

    private Vector3 _initPos;

    private void Start()
    {
        _initPos = transform.position;
    }

    private void Update()
    {
        float y = _initPos.y + Mathf.Sin(speed * Time.time) * height;
        transform.position = new Vector3(_initPos.x, y,_initPos.z);
    }
}
