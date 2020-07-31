using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Demo
{
    /// <summary>
    /// Demo script provided by Unity Community Wiki - wiki.unity3d.com
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFPSController : MonoBehaviour
    {
        public bool canJump = true;
        public float gravity = 10.0f;
        public float jumpHeight = 2.0f;
        public float maximumY = 60f;
        public float maxVelocityChange = 10.0f;
        public float minimumY = -60f;
        public float sensitivityX = 15f;
        public float sensitivityY = 15f;

        public float speed = 10.0f;

        private bool grounded;
        private Rigidbody rb;
        private float rotationY;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.useGravity = false;
        }

        private float CalculateJumpVerticalSpeed()
        {
            // From the jump height and gravity we deduce the upwards speed 
            // for the character to reach at the apex.
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }
        
        private bool PointerOverUI
        {
            get { return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(); }
        }

        private void LateUpdate()
        {
            if (Time.frameCount < 10f)
            {
                return;
            }
            
            if (grounded)
            {
                // Calculate how fast we should be moving
                Vector3 targetVelocity = new Vector3(UnityEngine.Input.GetAxis("Horizontal"), 0, UnityEngine.Input.GetAxis("Vertical"));
                targetVelocity = transform.TransformDirection(targetVelocity);
                targetVelocity *= speed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                Vector3 velocityChange = targetVelocity - velocity;
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                rb.AddForce(velocityChange, ForceMode.VelocityChange);

                // Jump
                if (canJump && UnityEngine.Input.GetKeyDown(KeyCode.Space))
                {
                    rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                }
            }

            // Rotation
            if (!PointerOverUI)
            {
                float timeFactor = Time.deltaTime * 20f;
                float rotationX = transform.localEulerAngles.y + UnityEngine.Input.GetAxis("Mouse X") * sensitivityX * timeFactor;
                rotationY += UnityEngine.Input.GetAxis("Mouse Y") * sensitivityY * timeFactor;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }

            // We apply gravity manually for more tuning control
            rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));

            grounded = false;
        }

        private void OnCollisionStay()
        {
            grounded = true;
        }
}
}
