using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     Destroys skidmark object when distance to the vehicle becomes greater then distance threshold.
    /// </summary>
    public class SkidmarkDestroy : MonoBehaviour
    {
        /// <summary>
        ///     Distance at which the GameObject will be destroyed.
        /// </summary>
        [Tooltip("    Distance at which the GameObject will be destroyed.")]
        public float distanceThreshold = 100f;

        public bool skidmarkIsBeingUsed;

        /// <summary>
        ///     Transform to which the object belongs to.
        /// </summary>
        [Tooltip("    Transform to which the object belongs to.")]
        public Transform targetTransform;

        private void Start()
        {
            InvokeRepeating("Check", Random.Range(1f, 2f), 1f);
        }

        private void Check()
        {
            if (targetTransform == null)
            {
                Destroy(gameObject);
            }
            else if (!skidmarkIsBeingUsed &&
                     Vector3.Distance(transform.position, targetTransform.position) > distanceThreshold)
            {
                Destroy(gameObject);
            }
        }
    }
}