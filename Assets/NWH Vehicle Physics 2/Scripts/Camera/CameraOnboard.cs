using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Cameras
{
    /// <summary>
    ///     Camera for on or in-vehicle use with option of head movement according to the G-forces.
    /// </summary>
    public class CameraOnboard : VehicleCamera
    {
        /// <summary>
        ///     Maximum head movement from the initial position.
        /// </summary>
        [FormerlySerializedAs("maxPositionOffsetMagnitude")]
        [Range(0f, 1f)]
        [Tooltip("    Maximum head movement from the initial position.")]
        public float maxMovementOffset = 0.2f;

        /// <summary>
        ///     How much will the head move around for the given g-force.
        /// </summary>
        [FormerlySerializedAs("positionIntensity")]
        [Range(0f, 1f)]
        [Tooltip("    How much will the head move around for the given g-force.")]
        public float movementIntensity = 0.125f;

        /// <summary>
        ///     Smoothing of the head movement.
        /// </summary>
        [FormerlySerializedAs("positionSmoothing")]
        [Range(0f, 1f)]
        [Tooltip("    Smoothing of the head movement.")]
        public float movementSmoothing = 0.3f;
        
        /// <summary>
        /// Movement intensity per axis. Set to 0 to disable movement on that axis or negative to reverse it.
        /// </summary>
        [UnityEngine.Tooltip("Movement intensity per axis. Set to 0 to disable movement on that axis or negative to reverse it.")]
        public Vector3 axisIntensity = new Vector3(1f, 0f, 1f);

        private Vector3 _accelerationChangeVelocity;
        private Vector3 _initialPosition;
        private Vector3 _localAcceleration;
        private Vector3 _newPositionOffset;
        private Vector3 _offsetChangeVelocity;
        private Vector3 _positionOffset;
        private Vector3 _prevAcceleration;
        private Transform _targetTransform;

        public override void Awake()
        {
            base.Awake();

            _targetTransform = target.transform;
            _initialPosition = _targetTransform.InverseTransformPoint(transform.position);
        }

        private void LateUpdate()
        {
            transform.position = _targetTransform.TransformPoint(_initialPosition);

            _localAcceleration = Vector3.zero;
            if (target != null)
            {
                _localAcceleration = _targetTransform.TransformDirection(target.Acceleration);
            }

            _newPositionOffset = Vector3.SmoothDamp(_prevAcceleration, _localAcceleration, ref _accelerationChangeVelocity,
                                    movementSmoothing) / 100f * movementIntensity;
            _newPositionOffset = Vector3.Scale(_newPositionOffset, axisIntensity);
            _positionOffset = Vector3.SmoothDamp(_positionOffset, _newPositionOffset, ref _offsetChangeVelocity,
                movementSmoothing);
            _positionOffset = Vector3.ClampMagnitude(_positionOffset, maxMovementOffset);
            transform.position -= _targetTransform.TransformDirection(_positionOffset) * Mathf.Clamp01(target.Speed * 0.5f);

            if (target != null)
            {
                _prevAcceleration = target.Acceleration;
            }
        }
    }
}