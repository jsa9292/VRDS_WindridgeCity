using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotateRigidbody : MonoBehaviour
{
    public float speed;
    public Vector3 axis;

    private float _angle;
    private Rigidbody _rb;

    private void Start()
    {
        _angle = 0;
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _angle = Time.deltaTime * speed;
        _rb.MoveRotation(transform.rotation * Quaternion.Euler(_angle * axis.x, _angle * axis.y, _angle * axis.z));
    }
}
