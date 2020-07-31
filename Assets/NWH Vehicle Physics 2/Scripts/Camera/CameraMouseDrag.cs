using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Cameras
{
    /// <summary>
    ///     Camera that can be dragged with the mouse.
    /// </summary>
    public class CameraMouseDrag : VehicleCamera
    {
        /// <summary>
        /// Can the camera be rotated by the user?
        /// </summary>
        [UnityEngine.Tooltip("Can the camera be rotated by the user?")]
        public bool allowRotation = true;

        /// <summary>
        /// Can the camera be panned by the user?
        /// </summary>
        [UnityEngine.Tooltip("Can the camera be panned by the user?")]
        public bool allowPanning = true;
        
        /// <summary>
        ///     Distance from target at which camera will be positioned. Might vary depending on smoothing.
        /// </summary>
        [Range(0, 100f)]
        [Tooltip("    Distance from target at which camera will be positioned. Might vary depending on smoothing.")]
        public float distance = 5f;

        /// <summary>
        ///     If true the camera will rotate with the vehicle.
        /// </summary>
        [Tooltip("    If true the camera will rotate with the vehicle.")]
        public bool followTargetsRotation;

        /// <summary>
        ///     Maximum distance that will be reached when zooming out.
        /// </summary>
        [Range(0, 100f)]
        [Tooltip("    Maximum distance that will be reached when zooming out.")]
        public float maxDistance = 13.0f;

        /// <summary>
        ///     Minimum distance that will be reached when zooming in.
        /// </summary>
        [Range(0, 100f)]
        [Tooltip("    Minimum distance that will be reached when zooming in.")]
        public float minDistance = 3.0f;

        /// <summary>
        ///     Sensitivity of the middle mouse button / wheel.
        /// </summary>
        [Range(0, 15)]
        [Tooltip("    Sensitivity of the middle mouse button / wheel.")]
        public float zoomSensitivity = 8.0f;

        /// <summary>
        ///     Smoothing of the camera rotation.
        /// </summary>
        [Range(0, 1)]
        [UnityEngine.Tooltip("    Smoothing of the camera rotation.")]
        public float rotationSmoothing = 0.25f;

        /// <summary>
        /// Maximum vertical angle the camera can achieve.
        /// </summary>
        [Range(-90, 90)]
        [UnityEngine.Tooltip("Maximum vertical angle the camera can achieve.")]
        public float verticalMaxAngle = 80.0f;

        /// <summary>
        /// Minimum vertical angle the camera can achieve.
        /// </summary>
        [Range(-90, 90)]
        [UnityEngine.Tooltip("Minimum vertical angle the camera can achieve.")]
        public float verticalMinAngle = -40.0f;

        /// <summary>
        /// Sensitivity of rotation input.
        /// </summary>
        [UnityEngine.Tooltip("Sensitivity of rotation input.")]
        public Vector2 rotationSensitivity = new Vector2(5f, 5f);
        
        /// <summary>
        /// Sensitivity of panning input.
        /// </summary>
        [UnityEngine.Tooltip("Sensitivity of panning input.")]
        public Vector2 panningSensitivity = new Vector2(0.06f, 0.06f);
        
        /// <summary>
        /// Initial rotation around the X axis (up/down)
        /// </summary>
        [UnityEngine.Tooltip("Initial rotation around the X axis (up/down)")]
        public float initXRotation;
        
        /// <summary>
        /// Initial rotation around the Y axis (left/right)
        /// </summary>
        [UnityEngine.Tooltip("Initial rotation around the Y axis (left/right)")]
        public float initYRotation;

        /// <summary>
        /// Look position offset from the target center.
        /// </summary>
        [UnityEngine.Tooltip("Look position offset from the target center.")]
        public Vector3 targetPositionOffset = Vector3.zero;

        private Vector3 _lookDir;
        private Vector3 _newLookDir;
        private Vector3 _lookDirVel;
        private Vector3 _camPosVel;
        private Vector3 _lookAtPosition;
        private Vector2 _rot;
        private Vector3 _pan;
        private Quaternion _lookAngle;
        private bool _isFirstFrame;

        private bool PointerOverUI
        {
            get { return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(); }
        }
        
        private void Start()
        {
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            _rot.x = initXRotation;
            _rot.y = initYRotation;
            _isFirstFrame = false;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            // Handle input
            if (!PointerOverUI)
            {
                float deadZone = 0.02f;
                if (allowRotation && UnityEngine.Input.GetMouseButton(0))
                {
                    _rot.y += UnityEngine.Input.GetAxis("Mouse X") * rotationSensitivity.x;
                    _rot.x -= UnityEngine.Input.GetAxis("Mouse Y") * rotationSensitivity.y;
                }
                if (allowPanning && UnityEngine.Input.GetMouseButton(1))
                {
                    float pX = UnityEngine.Input.GetAxis("Mouse X") * panningSensitivity.x;
                    float pY = UnityEngine.Input.GetAxis("Mouse Y") * panningSensitivity.y;
                    _pan -= targetTransform.InverseTransformDirection(transform.right * pX);
                    _pan -= targetTransform.InverseTransformDirection(transform.up * pY);
                }

                _rot.x = ClampAngle(_rot.x, verticalMinAngle, verticalMaxAngle);

                if (Mathf.Abs(UnityEngine.Input.GetAxis("Mouse ScrollWheel")) > deadZone)
                {
                    distance -= UnityEngine.Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
                }
            }
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Vector3 forwardVector = followTargetsRotation ? targetTransform.forward : Vector3.forward;
            Vector3 upVector = followTargetsRotation ? targetTransform.up : Vector3.up;
            Vector3 rightVector = followTargetsRotation ? targetTransform.right : Vector3.right;
            
            _lookAtPosition = targetTransform.position + targetTransform.TransformDirection(targetPositionOffset + _pan);
            _newLookDir = Quaternion.AngleAxis(_rot.x, rightVector) * forwardVector;
            _newLookDir =  Quaternion.AngleAxis(_rot.y, upVector) * _newLookDir;

            if (_isFirstFrame)
            {
                _lookDir = _newLookDir;
                _isFirstFrame = false;
            }
            else
            {
                _lookDir = Vector3.SmoothDamp(_lookDir, _newLookDir, ref _lookDirVel, rotationSmoothing);
            }

            transform.position = _lookAtPosition - _lookDir * distance;
            transform.forward = _lookDir;

            if (!followTargetsRotation)
            {
                transform.LookAt(_lookAtPosition);
            }
        }
 
        public void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(_lookAtPosition, 0.5f);
            Gizmos.DrawRay(_lookAtPosition, _lookDir);
        }

        public float ClampAngle(float angle, float min, float max)
        {
            while (angle < -360 || angle > 360)
            {
                if (angle < -360)
                {
                    angle += 360;
                }

                if (angle > 360)
                {
                    angle -= 360;
                }
            }

            return Mathf.Clamp(angle, min, max);
        }
    }
}