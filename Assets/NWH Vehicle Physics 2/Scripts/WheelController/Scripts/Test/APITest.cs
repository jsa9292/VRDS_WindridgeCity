using UnityEngine;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Class for testing API compatibility with Unity's wheel collider.
    /// </summary>
    public class APITest : MonoBehaviour
    {
        public float brakeTorque;
        public Vector3 center;
        public Friction forwardFriction;
        public WheelHit hit;
        public bool isGrounded;
        public float mass;
        public float motorTorque;
        public Vector3 position;
        public float radius;
        public Quaternion rotation;
        public float rpm;
        public Friction sidewaysFriction;
        public float steerAngle;
        public float suspensionDistance;
        public WheelController wheel;

        private void FixedUpdate()
        {
            // brakeTorque
            brakeTorque = wheel.brakeTorque;

            // center
            center = wheel.center;

            // forceAppPointDistance // not implemented

            // forwardFriction
            forwardFriction = wheel.forwardFriction;

            // isGrounded
            isGrounded = wheel.isGrounded;

            // mass
            mass = wheel.mass;

            // motorTorque
            motorTorque = wheel.motorTorque;

            // radius
            radius = wheel.radius;

            // rpm
            rpm = wheel.rpm;

            // sidewaysFriction
            sidewaysFriction = wheel.sideFriction;

            // sprungMass // not implemented, use wheel.suspensionForce instead.

            // steerAngle
            steerAngle = wheel.steerAngle;

            // suspensionDistance
            suspensionDistance = wheel.suspensionDistance;

            // suspensionSpring // not implemented, use wheel.spring instead

            // wheelDampingRate // use damper.reboundForce and damper.bumpForce

            // GetGroundHit()
            wheel.GetGroundHit(out WheelHit hit);

            // GetWorldPose()
            wheel.GetWorldPose(out position, out rotation);
        }
    }
}