using UnityEngine;

namespace NWH.VehiclePhysics2.Cameras
{
    /// <summary>
    ///     Camera that follows behind the vehicle.
    /// </summary>
    public class CameraFollow : VehicleCamera
    {
        /// <summary>
        ///     Distance at which camera will follow.
        /// </summary>
        [Range(0, 30f)]
        [Tooltip("    Distance at which camera will follow.")]
        public float distance = 5f;

        /// <summary>
        ///     Height in relation to the target at which the camera will follow.
        /// </summary>
        [Range(0, 10f)]
        [Tooltip("    Height in relation to the target at which the camera will follow.")]
        public float height = 2.5f;

        /// <summary>
        ///     Positional and rotational smoothing of the camera.
        /// </summary>
        [Range(0, 1f)]
        [Tooltip("    Positional and rotational smoothing of the camera.")]
        public float smoothing = 0.2f;

        /// <summary>
        ///     Offset in the forward direction from the target. Use this if you do not want to use camera baits.
        /// </summary>
        [Range(-10f, 10f)]
        [Tooltip(
            "    Offset in the forward direction from the target. Use this if you do not want to use camera baits.")]
        public float targetForwardOffset;

        /// <summary>
        ///     Offset in the up direction from the target. Use this if you do not want to use camera baits.
        /// </summary>
        [Range(-5, 5f)]
        [Tooltip("    Offset in the up direction from the target. Use this if you do not want to use camera baits.")]
        public float targetUpOffset = 1.25f;

        private bool firstFrame = true;

        private Vector3 targetForward;

        private Vector3 targetForwardVelocity;

        private void LateUpdate()
        {
            Vector3 prevTargetForward = targetForward;

            if (!firstFrame)
            {
                targetForward = Vector3.SmoothDamp(prevTargetForward, target.transform.forward, ref targetForwardVelocity,
                    smoothing);
            }
            else
            {
                targetForward = target.transform.forward;
                firstFrame = false;
            }

            Vector3 desiredPosition = target.transform.position + targetForward * -distance + Vector3.up * height;

            // Check for ground
            RaycastHit hit;
            if (Physics.Raycast(desiredPosition, -Vector3.up, out hit, 0.8f))
            {
                desiredPosition = hit.point + Vector3.up * 0.8f;
            }

            transform.position = desiredPosition;
            transform.LookAt(target.transform.position + Vector3.up * targetUpOffset + target.transform.forward * targetForwardOffset);
        }

        private void OnEnable()
        {
            firstFrame = true;
        }
    }
}