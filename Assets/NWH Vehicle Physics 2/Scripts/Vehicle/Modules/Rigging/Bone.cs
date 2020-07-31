using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Rigging
{
    [Serializable]
    public class Bone
    {
        /// <summary>
        ///     Should the object be positioned between two points. Also affects position besides rotation.
        /// </summary>
        [Tooltip("    Should the object be positioned between two points. Also affects position besides rotation.")]
        public bool doubleSided;

        /// <summary>
        ///     Should the object be rotated to look at the target?
        /// </summary>
        [Tooltip("    Should the object be rotated to look at the target?")]
        public bool lookAtTarget = true;

        /// <summary>
        ///     Should the object be stretched between pivot and target?
        /// </summary>
        [Tooltip("    Should the object be stretched between pivot and target?")]
        public bool stretchToTarget = true;

        /// <summary>
        ///     The transform that represents the lookAtTarget and stretch target.
        /// </summary>
        [Tooltip("    The transform that represents the lookAtTarget and stretch target.")]
        public Transform targetTransform;

        /// <summary>
        ///     Second target. Object will be stretched positioned between targetTransform and targetTransformB if doubleSided is
        ///     true.
        /// </summary>
        [Tooltip(
            "    Second target. Object will be stretched positioned between targetTransform and targetTransformB if doubleSided is\r\n    true.")]
        public Transform targetTransformB;

        /// <summary>
        ///     The transform that represents the bone.
        /// </summary>
        [Tooltip("    The transform that represents the bone.")]
        public Transform thisTransform;

        private float _initDistance;
        private float _initZScale;

        public void Initialize()
        {
            _initDistance = Vector3.Distance(thisTransform.position, targetTransform.position);
            _initZScale = thisTransform.localScale.z;
        }

        public void Update(Vector3 forward, Vector3 up)
        {
            if (doubleSided)
            {
                Vector3 position = (targetTransform.position + targetTransformB.position) / 2f;
                thisTransform.position = position;
                thisTransform.LookAt(targetTransform, up);
                return;
            }

            if (lookAtTarget)
            {
                Vector3 rot = Quaternion.LookRotation(targetTransform.position - thisTransform.position, up)
                    .eulerAngles;
                thisTransform.rotation = Quaternion.Euler(rot);
            }

            if (stretchToTarget && _initDistance != 0)
            {
                float distance = Vector3.Distance(thisTransform.position, targetTransform.position);
                float newZScale = distance / _initDistance * _initZScale;
                Vector3 scale = thisTransform.localScale;
                thisTransform.localScale = new Vector3(scale.x, scale.y, newZScale);
            }
        }
    }
}