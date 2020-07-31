using UnityEngine;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Represents single ground ray hit.
    /// </summary>
    public class WheelHit
    {
        public float angleForward;
        public float curvatureOffset;
        public float distanceFromTire;

        /// <summary>
        ///     The magnitude of the force being applied for the contact. [N]
        /// </summary>
        [Tooltip("    The magnitude of the force being applied for the contact. [N]")]
        public float force;

        /// <summary>
        ///     The direction the wheel is pointing in.
        /// </summary>
        [Tooltip("    The direction the wheel is pointing in.")]
        public Vector3 forwardDir;

        /// <summary>
        ///     Tire slip in the rolling direction.
        /// </summary>
        [Tooltip("    Tire slip in the rolling direction.")]
        public float forwardSlip;

        public Vector3 groundPoint;
        public Vector2 offset;

        [SerializeField]
        public RaycastHit raycastHit;

        /// <summary>
        ///     The sideways direction of the wheel.
        /// </summary>
        [Tooltip("    The sideways direction of the wheel.")]
        public Vector3 sidewaysDir;

        /// <summary>
        ///     The slip in the sideways direction.
        /// </summary>
        [Tooltip("    The slip in the sideways direction.")]
        public float sidewaysSlip;

        public bool valid = false;

        public float weight;

        // WheelCollider compatibility variables
        public Collider collider
        {
            get { return raycastHit.collider; }
        }

        /// <summary>
        ///     The normal at the point of contact
        /// </summary>
        public Vector3 normal
        {
            get { return raycastHit.normal; }
        }

        /// <summary>
        ///     The point of contact between the wheel and the ground.
        /// </summary>
        public Vector3 point
        {
            get { return groundPoint; }
        }
    }
}