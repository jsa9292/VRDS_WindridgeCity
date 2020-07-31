using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Cameras
{
    public class VehicleCamera : MonoBehaviour
    {
        /// <summary>
        ///     Vehicle Controller that this script is targeting. Can be left empty if head movement is not being used.
        /// </summary>
        [Tooltip(
            "Vehicle Controller that this script is targeting. Can be left empty if head movement is not being used.")]
        public VehicleController target;

        /// <summary>
        /// Transform of the target object.
        /// </summary>
        [UnityEngine.Tooltip("Transform of the target object.")]
        public Transform targetTransform;

        public virtual void Awake()
        {
            if (target == null)
            {
                target = transform.GetComponentInParent<VehicleController>();
                if (target == null)
                {
                    Debug.LogError($"No parent object of VehicleCamera {name} has VehicleController component. Make sure the VehicleCamera is " +
                                   "a child of a vehicle.");
                }
            }
            
            targetTransform = target.transform;
        }
    }
}