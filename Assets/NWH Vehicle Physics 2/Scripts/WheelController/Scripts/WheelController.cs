using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Main class of WheelController package that controls all the aspects of the wheel. Roughly equivalent to Unity's
    ///     WheelCollider.
    /// </summary>
    [Serializable]
    public class WheelController : MonoBehaviour
    {
        /// <summary>
        ///     Side of the vehicle.
        /// </summary>
        public enum Side
        {
            Left = -1,
            Right = 1,
            Center = 0,
            Auto = 2
        }

        /// <summary>
        ///     Current active friction preset.
        /// </summary>
        [Tooltip("    Current active friction preset.")]
        public FrictionPreset activeFrictionPreset;

        /// <summary>
        ///     Should forces be applied to other rigidbodies when wheel is in contact with them?
        /// </summary>
        [Tooltip("    Should forces be applied to other rigidbodies when wheel is in contact with them?")]
        public bool applyForceToOthers;

        public bool autoSetupLayerMask = true;

        /// <summary>
        ///     Cached value of this.transform.
        /// </summary>
        [Tooltip("    Cached value of this.transform.")]
        public Transform cachedTransform;

        /// <summary>
        ///     Cached value of visual's transform.
        /// </summary>
        [Tooltip("    Cached value of visual's transform.")]
        public Transform cachedVisualTransform;

        /// <summary>
        ///     Instance of the damper.
        /// </summary>
        [SerializeField]
        [Tooltip("    Instance of the damper.")]
        public Damper damper;

        /// <summary>
        ///     If set to true draws detailed debug info.
        /// </summary>
        [Tooltip("    If set to true draws detailed debug info.")]
        public bool debug;

        /// <summary>
        ///     Constant torque acting similar to brake torque.
        ///     Imitates rolling resistance.
        /// </summary>
        [Range(0, 200)]
        [Tooltip("    Constant torque acting similar to brake torque.\r\n    Imitates rolling resistance.")]
        public float dragTorque = 10f;

        /// <summary>
        ///     Forward (longitudinal) friction info.
        /// </summary>
        [FormerlySerializedAs("fFriction")]
        [Tooltip("    Forward (longitudinal) friction info.")]
        public Friction forwardFriction;

        /// <summary>
        ///     True if wheel touching ground.
        /// </summary>
        [Tooltip("    True if wheel touching ground.")]
        public bool hasHit = true;

        public LayerMask layerMask = Physics.IgnoreRaycastLayer;

        /// <summary>
        ///     Curve where X axis represents tire load as a percentage [0,1] of maximumTireLoad and Y axis
        ///     represents tire grip force as a percentage [0,1] of maximumTireGripForce.
        ///     Drastically influences handling.
        /// </summary>
        [Tooltip(
            "Curve where X axis represents tire load as a percentage [0,1] of maximumTireLoad and Y axis\r\nrepresents tire grip force as a percentage [0,1] of maximumTireGripForce.\r\nDrastically influences handling.")]
        public AnimationCurve loadGripCurve = new AnimationCurve
        {
            keys = new[]
            {
                new Keyframe(0f, 0f, 0, 1f),
                new Keyframe(0.35f, 0.6f, 1f, 1f),
                new Keyframe(1f, 1f)
            }
        };

        /// <summary>
        ///     Maximum total force a tire can exert on surface, no matter the load.
        /// </summary>
        [Tooltip("    Maximum total force a tire can exert on surface, no matter the load.")]
        public float maximumTireGripForce = 10200f;

        /// <summary>
        ///     Tire load at which the grip force reaches it's maximum.
        /// </summary>
        [Tooltip("    Tire load at which the grip force reaches it's maximum.")]
        public float maximumTireLoad = 9600f;

        /// <summary>
        ///     Root object of the vehicle.
        /// </summary>
        [SerializeField]
        [Tooltip("    Root object of the vehicle.")]
        public GameObject parent;

        /// <summary>
        ///     Rigidbody to which the forces will be applied.
        /// </summary>
        [Tooltip("    Rigidbody to which the forces will be applied.")]
        public Rigidbody parentRigidbody;

        /// <summary>
        ///     Side (lateral) friction info.
        /// </summary>
        [FormerlySerializedAs("sFriction")]
        [Tooltip("    Side (lateral) friction info.")]
        public Friction sideFriction;

        /// <summary>
        ///     When enabled only a single raycast is used to detect ground.
        ///     Very fast and should be used when performance is critical.
        /// </summary>
        [Tooltip(
            "When enabled only a single raycast is used to detect ground.\r\nVery fast and should be used when performance is critical.")]
        public bool singleRay;

        /// <summary>
        ///     Instance of the spring.
        /// </summary>
        [SerializeField]
        [Tooltip("    Instance of the spring.")]
        public Spring spring;

        // Amount of torque transferred from the wheel to the chassis of the vehicle. 
        // Lower values for vehicles that have anti-squat. Vehicle with wheel fixed at center directly to the chassis would have
        // value of 1f, while depending on rear suspension configuration this value can be <0 (rear end of the vehicle rises instead of squats on accelerationMag).
        // Small amount of squat is recommended on RWD cars as this loads the rear tires more giving the vehicle more traction.
        [Range(-1, 1f)]
        [Tooltip(
            "Amount of torque transferred from the wheel to the chassis of the vehicle. \r\nLower values for vehicles that have anti-squat. Vehicle with wheel fixed at center directly to the chassis would have\r\nvalue of 1f, while depending on rear suspension configuration this value can be <0 (rear end of the vehicle rises instead of squats on accelerationMag).\r\nSmall amount of squat is recommended on RWD cars as this loads the rear tires more giving the vehicle more traction.")]
        public float squat = 0.2f;

        public bool useExternalLatSlipCalculation;
        public bool useExternalLongSlipCalculation;

        /// <summary>
        ///     When true Step() will not be called each FixedUpdate().
        ///     Used when execution order is important and/or the other script is waiting on the result of Step().
        /// </summary>
        [Tooltip(
            "When true Step() will not be called each FixedUpdate().\r\nUsed when execution order is important and/or the other script is waiting on the result of Step().")]
        public bool useExternalUpdate;

        /// <summary>
        ///     If enabled mesh collider mimicking the shape of rim and wheel will be positioned so that wheel can not pass through
        ///     objects in case raycast does not detect the surface in time.
        /// </summary>
        [Tooltip(
            "If enabled mesh collider mimicking the shape of rim and wheel will be positioned so that wheel can not pass through\r\nobjects in case raycast does not detect the surface in time.")]
        public bool useRimCollider = true;

        /// <summary>
        ///     Side the wheel is on.
        /// </summary>
        [SerializeField]
        [Tooltip("    Side the wheel is on.")]
        public Side vehicleSide = Side.Auto;

        public int vehicleWheelCount;

        public bool visualOnlyUpdate;

        /// <summary>
        ///     Instance of the wheel.
        /// </summary>
        [SerializeField]
        [Tooltip("    Instance of the wheel.")]
        public Wheel wheel;

        /// <summary>
        ///     Contains point in which wheel touches ground. Not valid if !isGrounded.
        /// </summary>
        [Tooltip("    Contains point in which wheel touches ground. Not valid if !isGrounded.")]
        public WheelHit wheelHit = new WheelHit();

        private Vector3 _alternateForwardNormal;
        private Quaternion _axleRotation;
        private float _bottomOutDistance;

        private float _boundsX, _boundsY, _boundsZ, _boundsW;
        private Vector3 _contactVelocity;

        private float _damage;
        private float _fixedDeltaTime;
        private Vector3 _hitDir;
        private Vector3 _hitPointSum = Vector3.zero;
        private bool _initialized;
        private int _minDistRayIndex;
        private Vector3 _normal;
        private Vector3 _normalSum = Vector3.zero;
        private Vector3 _offsetPrecalc;

        private Vector3 _origin;
        private Vector3 _point;
        private float _prevBottomOutDistance;
        private Vector3 _prevMpPosition;
        private float _prevRadius;
        private float _prevWidth;
        private NativeArray<RaycastCommand> _raycastCommands;
        private RaycastCommand[] _raycastCommandsArray;
        private Vector3 _raycastHitNormal;

        // Raycast command
        private NativeArray<RaycastHit> _raycastHits;
        private RaycastHit[] _raycastHitsArray;
        private JobHandle _raycastJobHandle;
        private float _rayLength;
        private Vector3 _scale;
        private float _stepX, _stepY;
        private Vector3 _surfaceForceVector;
        private Vector3 _transformForward;
        private Vector3 _transformPosition;
        private Vector3 _transformRight;
        private Quaternion _transformRotation;
        private Vector3 _transformUp;
        private float _weight;
        private Vector3 _wheelDown;

        private Vector3 _wheelHitPoint;
        private float _yScale;
        private Quaternion camberQuaternion;

        private bool hasBeenEnabledThisFrame = false;

        /// <summary>
        ///     Number of raycasts in the side / lateral direction.
        /// </summary>
        [SerializeField]
        private int lateralScanResolution = 3; // number of scan planes (side-to-side)

        /// <summary>
        ///     Number of raycasts in the forward / longitudinal direction.
        /// </summary>
        [SerializeField]
        private int longitudinalScanResolution = 8; // axisResolution of the first scan pass


        private WheelHit singleWheelHit = new WheelHit();

        // Wheel rotation
        private Quaternion steerQuaternion;

        [SerializeField]
        private float suspensionForceMagnitude;

        private Quaternion totalRotation;

        /// <summary>
        ///     Array of rays and related data that are shot each frame to detect surface features.
        ///     Contains offsets, hit points, normals, etc. of each point.
        /// </summary>
        [SerializeField]
        private WheelHit[] wheelHits;

        public float Damage
        {
            get { return _damage; }
            set { ApplyDamage(value); }
        }

        /// <summary>
        ///     Returns angular velocity of the wheel in radians. Multiply by wheel radius to get linear speed.
        /// </summary>
        public float angularVelocity
        {
            get { return wheel.angularVelocity; }
            set { wheel.angularVelocity = value; }
        }

        /// <summary>
        ///     Brake torque on the wheel axle. [Nm]
        ///     Must be positive (zero included).
        /// </summary>
        public float brakeTorque
        {
            get { return wheel.brakeTorque; }
            set
            {
                if (value >= 0)
                {
                    wheel.brakeTorque = value;
                }
                else
                {
                    wheel.brakeTorque = 0;
                    Debug.LogWarning("Brake torque must be positive. Received <0.");
                }
            }
        }

        /// <summary>
        ///     Camber angle of the wheel. [deg]
        ///     Negative angle means that the top of the wheel in closer to the vehicle than the bottom.
        /// </summary>
        public float camber
        {
            get { return wheel.camberAngle; }
        }

        /// <summary>
        ///     The center of the wheel, measured in the world space.
        /// </summary>
        public Vector3 center
        {
            get { return cachedTransform.InverseTransformPoint(worldCenter); }
        }

        /// <summary>
        ///     Bump force at 1 m/s spring velocity
        /// </summary>
        public float damperBumpForce
        {
            get { return damper.bumpForce; }
            set { damper.bumpForce = value; }
        }

        /// <summary>
        ///     Damper force curve in relation to spring velocity.
        /// </summary>
        public AnimationCurve DamperCurve
        {
            get { return damper.curve; }
            set { damper.curve = value; }
        }

        /// <summary>
        ///     Current damper force.
        /// </summary>
        public float damperForce
        {
            get { return damper.force; }
        }

        /// <summary>
        ///     Rebounding force at 1 m/s spring velocity
        /// </summary>
        public float damperReboundForce
        {
            get { return damper.reboundForce; }
            set { damper.reboundForce = value; }
        }

        /// <summary>
        ///     Ground scan axisResolution in forward direction.
        /// </summary>
        public int ForwardScanResolution
        {
            get { return longitudinalScanResolution; }
            set
            {
                longitudinalScanResolution = value;

                if (longitudinalScanResolution < 1)
                {
                    longitudinalScanResolution = 1;
                    Debug.LogWarning("Forward scan axisResolution must be > 0.");
                }

                InitializeScanParams();
            }
        }

        /// <summary>
        ///     Is the tractive surface touching the ground?
        ///     Returns false if vehicle tipped over / tire sidewall is in contact.
        /// </summary>
        public bool isGrounded
        {
            get { return hasHit; }
        }

        /// <summary>
        ///     Only layers with value of 1 (ticked) will get detected by the wheel.
        /// </summary>
        public LayerMask LayerMask
        {
            get { return layerMask; }

            set { layerMask = value; }
        }

        /// <summary>
        ///     Mass of the wheel. [kg]
        ///     Typical values would be in range [20, 200]
        /// </summary>
        public float mass
        {
            get { return wheel.mass; }
            set { wheel.mass = value; }
        }

        /// <summary>
        ///     Motor torque on the wheel axle. [Nm]
        ///     Can be positive or negative based on direction.
        /// </summary>
        public float motorTorque
        {
            get { return wheel.motorTorque; }
            set { wheel.motorTorque = value; }
        }

        /// <summary>
        ///     Object that follows the wheel position in everything but rotation around the axle.
        ///     Can be used for brake calipers, external fenders, etc.
        /// </summary>
        public GameObject NonRotatingVisual
        {
            get { return wheel.NonRotatingVisual; }
            set { wheel.NonRotatingVisual = value; }
        }

        /// <summary>
        ///     Returns wheel's parent object.
        /// </summary>
        public GameObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        ///     Returns velocity at the wheel's center position in [m/s].
        /// </summary>
        public Vector3 pointVelocity
        {
            get { return parentRigidbody.GetPointVelocity(wheel.worldPosition); }
        }

        /// <summary>
        ///     Radius (height) of the tire. [meters]
        /// </summary>
        public float radius
        {
            get { return wheel.radius; }
            set
            {
                wheel.radius = value;
                InitializeScanParams();
            }
        }

        /// <summary>
        ///     Side offset of the rim. Positive value will result if wheel further from the vehicle. [meters]
        /// </summary>
        public float rimOffset
        {
            get { return wheel.rimOffset; }
            set { wheel.rimOffset = value; }
        }

        /// <summary>
        ///     Rotations per minute of the wheel around the axle. [RPM]
        /// </summary>
        public float rpm
        {
            get { return wheel.RPM; }
        }

        /// <summary>
        ///     Maximum extension distance of wheel suspension, measured in local space.
        ///     Same as spring.maxLength
        /// </summary>
        public float suspensionDistance
        {
            get { return spring.maxLength; }
            set { spring.maxLength = value; }
        }

        /// <summary>
        ///     Number of scan planes parallel to the wheel.
        /// </summary>
        public int SideToSideScanResolution
        {
            get { return lateralScanResolution; }
            set
            {
                lateralScanResolution = value;
                if (lateralScanResolution < 1)
                {
                    lateralScanResolution = 1;
                    Debug.LogWarning("Side to side scan axisResolution must be > 0.");
                }

                InitializeScanParams();
            }
        }

        /// <summary>
        ///     Returns vehicle speed in meters per second [m/s], multiply by 3.6 for [kph] or by 2.24 for [mph].
        /// </summary>
        public float speed
        {
            get { return forwardFriction.speed; }
        }

        /// <summary>
        ///     True when spring is fully compressed, i.e. there is no more spring travel.
        /// </summary>
        public bool springBottomedOut
        {
            get { return spring.bottomedOut; }
        }

        /// <summary>
        ///     Returns value in range [0,1] where 1 means spring is fully compressed.
        /// </summary>
        public float springCompression
        {
            get { return 1f - spring.compressionPercent; }
        }

        /// <summary>
        ///     Spring force curve in relation to spring length.
        /// </summary>
        public AnimationCurve springCurve
        {
            get { return spring.forceCurve; }
            set { spring.forceCurve = value; }
        }

        /// <summary>
        ///     Length of the spring when fully extended.
        /// </summary>
        public float springLength
        {
            get { return spring.maxLength; }
            set { spring.maxLength = value; }
        }

        /// <summary>
        ///     Maximum spring force. [N]
        /// </summary>
        public float springMaximumForce
        {
            get { return spring.maxForce; }
            set { spring.maxForce = value; }
        }

        /// <summary>
        ///     True when spring is fully extended.
        /// </summary>
        public bool springOverExtended
        {
            get { return spring.overExtended; }
        }

        /// <summary>
        ///     Current length (travel) of spring.
        /// </summary>
        public float springTravel
        {
            get { return spring.length; }
        }

        /// <summary>
        ///     Point in which spring and swingarm are in contact.
        /// </summary>
        public Vector3 springTravelPoint
        {
            get { return cachedTransform.position - cachedTransform.up * spring.length; }
        }


        /// <summary>
        ///     Spring velocity in relation to local vertical axis. [m/s]
        ///     Positive on rebound (extension), negative on bump (compression).
        /// </summary>
        public float springVelocity
        {
            get { return spring.velocity; }
        }


        /// <summary>
        ///     Steer angle around the wheel's up axis (with add-ons ignored). [deg]
        /// </summary>
        public float steerAngle
        {
            get { return wheel.steerAngle; }
            set { wheel.steerAngle = value; }
        }

        /// <summary>
        ///     Current spring force. [N]
        ///     Can be written to for use in Anti-roll Bar script or similar.
        /// </summary>
        public float suspensionForce
        {
            get { return spring.force; }
            set { spring.force = value; }
        }

        /// <summary>
        ///     Returns Enum [Side] with the corresponding side of the vehicle a wheel is at [Left, Right]
        /// </summary>
        public Side VehicleSide
        {
            get { return vehicleSide; }
            set { vehicleSide = value; }
        }

        /// <summary>
        ///     Returns object that represents wheel's visual representation. Can be null in case the object is not assigned (not
        ///     mandatory).
        /// </summary>
        public GameObject Visual
        {
            get { return wheel.Visual; }
            set { wheel.Visual = value; }
        }

        /// <summary>
        ///     Width of the wheel. [meters]
        /// </summary>
        public float width
        {
            get { return wheel.width; }
            set
            {
                wheel.width = value;
                InitializeScanParams();
            }
        }

        /// <summary>
        ///     The center of the wheel, measured in the world space.
        /// </summary>
        public Vector3 worldCenter
        {
            get { return _transformPosition - _transformUp * spring.length; }
        }

        public void Initialize()
        {
            _fixedDeltaTime = Time.fixedDeltaTime;

            // Cache transform
            cachedTransform = transform;

            SetDefaults();

            // Set the world position to the position of the wheel
            if (wheel.Visual != null)
            {
                cachedVisualTransform = wheel.Visual.transform;
                wheel.worldPosition = cachedVisualTransform.position;
                wheel.up = cachedVisualTransform.up;
                wheel.forward = cachedVisualTransform.forward;
                wheel.right = cachedVisualTransform.right;
            }

            if (wheel.NonRotatingVisual != null)
            {
                wheel.nonRotatingPositionOffset =
                    wheel.Visual.transform.InverseTransformDirection(
                        wheel.NonRotatingVisual.transform.position - cachedVisualTransform.position);
            }

            // Initialize the wheel params
            wheel.Initialize(this);

            InitializeScanParams();

            // Find parent
            parentRigidbody = parent.GetComponent<Rigidbody>();

            // Initialize spring length to starting value.
            spring.length = spring.maxLength * 0.5f * _yScale;

            _prevRadius = wheel.radius;
            _prevWidth = wheel.width;

            _initialized = true;

            vehicleWheelCount = cachedTransform.parent.GetComponentsInChildren<WheelController>().Length;

            forwardFriction.Initialize();
            sideFriction.Initialize();
        }

        /// <summary>
        ///     Sets up coordinates, arrays and other fields for ground detection.
        ///     Needs to be called each time a dimension of the wheel or ground detection axisResolution changes.
        /// </summary>
        public void InitializeScanParams()
        {
            // Scan start point
            _boundsX = -wheel.width / 2f;
            _boundsY = -wheel.radius;

            // Scan end point
            _boundsZ = wheel.width / 2f + 0.000001f;
            _boundsW = wheel.radius + 0.000001f;

            // Increment
            _stepX = lateralScanResolution == 1 ? 1 : wheel.width / (lateralScanResolution - 1);
            _stepY = longitudinalScanResolution == 1 ? 1 : wheel.radius * 2f / (longitudinalScanResolution - 1);

            // Initialize wheel rays
            int n = longitudinalScanResolution * lateralScanResolution;
            wheelHits = new WheelHit[n];

            int w = 0;
            for (float x = _boundsX; x <= _boundsZ; x += _stepX)
            {
                int h = 0;
                for (float y = _boundsY; y <= _boundsW; y += _stepY)
                {
                    int index = w * longitudinalScanResolution + h;

                    WheelHit wr = new WheelHit();
                    wr.angleForward = Mathf.Asin(y / (wheel.radius + 0.000001f));
                    wr.curvatureOffset = Mathf.Cos(wr.angleForward) * wheel.radius;

                    float xOffset = x;
                    if (lateralScanResolution == 1)
                    {
                        xOffset = 0;
                    }

                    wr.offset = new Vector2(xOffset, y);
                    wheelHits[index] = wr;

                    h++;
                }

                w++;
            }

            if (_raycastCommands.IsCreated)
            {
                _raycastCommands.Dispose();
            }

            if (_raycastHits.IsCreated)
            {
                _raycastHits.Dispose();
            }

            GenerateRaycastArraysIfNeeded(n);
        }

        private void Awake()
        {
            Initialize();
        }

        private void FixedUpdate()
        {
            _fixedDeltaTime = Time.fixedDeltaTime;

            if (!useExternalUpdate)
            {
                Step();
            }

            hasBeenEnabledThisFrame = false;
        }

        private void OnEnable()
        {
            hasBeenEnabledThisFrame = true;
        }

        private void UpdateCachedValues()
        {
            _transformPosition = cachedTransform.position;
            _transformRotation = cachedTransform.rotation;
            _transformForward = cachedTransform.forward;
            _transformRight = cachedTransform.right;
            _transformUp = cachedTransform.up;
        }


        /// <summary>
        ///     Searches for wheel hit point by iterating WheelScan() function to the requested scan depth.
        /// </summary>
        private void HitUpdate()
        {
            // Hit flag     
            float minDistance = 9999999f;
            _wheelDown = -wheel.up;

            float distanceThreshold = spring.maxLength - spring.length;
            _rayLength = wheel.radius * 2.1f + distanceThreshold;

            _offsetPrecalc.x = _transformPosition.x - _transformUp.x * spring.length + wheel.up.x * wheel.radius -
                               wheel.inside.x * wheel.rimOffset;
            _offsetPrecalc.y = _transformPosition.y - _transformUp.y * spring.length + wheel.up.y * wheel.radius -
                               wheel.inside.y * wheel.rimOffset;
            _offsetPrecalc.z = _transformPosition.z - _transformUp.z * spring.length + wheel.up.z * wheel.radius -
                               wheel.inside.z * wheel.rimOffset;

            int validHitCount = 0;
            _minDistRayIndex = -1;
            hasHit = false;

            if (singleRay)
            {
                singleWheelHit.valid = false;

                bool grounded = Physics.Raycast(_offsetPrecalc, _wheelDown, out singleWheelHit.raycastHit,
                    _rayLength + wheel.radius, layerMask);

                if (grounded)
                {
                    float distanceFromTire = singleWheelHit.raycastHit.distance - wheel.radius - wheel.radius;
                    if (distanceFromTire > distanceThreshold)
                    {
                        return;
                    }

                    singleWheelHit.valid = true;
                    hasHit = true;
                    singleWheelHit.distanceFromTire = distanceFromTire;

                    wheelHit.raycastHit = singleWheelHit.raycastHit;
                    wheelHit.angleForward = singleWheelHit.angleForward;
                    wheelHit.distanceFromTire = singleWheelHit.distanceFromTire;
                    wheelHit.offset = singleWheelHit.offset;
                    wheelHit.weight = singleWheelHit.weight;
                    wheelHit.curvatureOffset = singleWheelHit.curvatureOffset;

                    wheelHit.groundPoint = wheelHit.raycastHit.point;
                    wheelHit.raycastHit.point += wheel.up * wheel.radius;
                    wheelHit.curvatureOffset = wheel.radius;
                }
            }
            else
            {
                int n = wheelHits.Length;
                GenerateRaycastArraysIfNeeded(n);

                for (int i = 0; i < n; i++)
                {
                    wheelHits[i].valid = false;

                    Vector3 offset = wheelHits[i].offset;
                    _origin.x = wheel.forward.x * offset.y + wheel.right.x * offset.x + _offsetPrecalc.x;
                    _origin.y = wheel.forward.y * offset.y + wheel.right.y * offset.x + _offsetPrecalc.y;
                    _origin.z = wheel.forward.z * offset.y + wheel.right.z * offset.x + _offsetPrecalc.z;

                    _raycastCommandsArray[i].from = _origin;
                    _raycastCommandsArray[i].direction = _wheelDown;
                    _raycastCommandsArray[i].distance = _rayLength + wheelHits[i].curvatureOffset;
                    _raycastCommandsArray[i].layerMask = layerMask;
                    _raycastCommandsArray[i].maxHits = 1;
                }

                _raycastCommands.CopyFrom(_raycastCommandsArray);
                _raycastJobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, 8);
                _raycastJobHandle.Complete();
                _raycastHits.CopyTo(_raycastHitsArray);

                for (int i = 0; i < n; i++)
                {
                    wheelHits[i].valid = false;

                    if (_raycastHitsArray[i].distance > 0)
                    {
                        float distanceFromTire =
                            _raycastHitsArray[i].distance - wheelHits[i].curvatureOffset - wheel.radius;

                        if (distanceFromTire > distanceThreshold)
                        {
                            continue;
                        }

                        wheelHits[i].valid = true;
                        hasHit = true;
                        wheelHits[i].raycastHit = _raycastHitsArray[i];
                        wheelHits[i].distanceFromTire = distanceFromTire;

                        validHitCount++;

                        if (distanceFromTire < minDistance)
                        {
                            minDistance = distanceFromTire;
                            _minDistRayIndex = i;
                        }
                    }
                }

                if (hasHit)
                {
                    CalculateAverageWheelHit();
                }
            }

            // Friction force directions
            if (hasHit)
            {
                wheelHit.forwardDir = Vector3.Normalize(Vector3.Cross(wheelHit.normal, -wheel.right));
                wheelHit.sidewaysDir = Quaternion.AngleAxis(90f, wheelHit.normal) * wheelHit.forwardDir;
            }
        }


        private void SuspensionUpdate()
        {
            if (hasHit)
            {
                spring.bottomedOut = spring.overExtended = false;

                // Calculate spring length from ground hit, position of the wheel and transform position.     
                spring.bottomedOut = spring.overExtended = false;

                // Calculate spring length from ground hit, position of the wheel and transform position.     
                Vector3 hitPoint = wheelHit.raycastHit.point;
                float rimOffset = wheel.rimOffset * (int) vehicleSide;

                if (singleRay)
                {
                    Vector3 correctedHitPoint = wheelHit.raycastHit.point - _transformUp * (wheel.radius * 0.06f);
                    spring.targetPoint.x = correctedHitPoint.x - wheel.right.x * rimOffset;
                    spring.targetPoint.y = correctedHitPoint.y - wheel.right.y * rimOffset;
                    spring.targetPoint.z = correctedHitPoint.z - wheel.right.z * rimOffset;
                }
                else
                {
                    spring.targetPoint.x = hitPoint.x - wheel.forward.x * wheelHit.offset.y
                                                      - wheel.right.x * wheelHit.offset.x - wheel.right.x * rimOffset;
                    spring.targetPoint.y = hitPoint.y - wheel.forward.y * wheelHit.offset.y
                                                      - wheel.right.y * wheelHit.offset.x - wheel.right.y * rimOffset;
                    spring.targetPoint.z = hitPoint.z - wheel.forward.z * wheelHit.offset.y
                                                      - wheel.right.z * wheelHit.offset.x - wheel.right.z * rimOffset;
                }

                spring.length = -cachedTransform.InverseTransformPoint(spring.targetPoint).y * _yScale;

                if (spring.length < 0f)
                {
                    _bottomOutDistance = -spring.length;
                    spring.length = 0f;
                    spring.bottomedOut = true;
                }
                else if (spring.length > spring.maxLength)
                {
                    spring.length = spring.maxLength;
                    spring.overExtended = true;
                }
            }
            else
            {
                // If the wheel suddenly gets in the air smoothly extend it.
                spring.length = Mathf.Lerp(spring.length, spring.maxLength, _fixedDeltaTime * 10f);
                damper.force = 0;
            }

            if (hasBeenEnabledThisFrame)
            {
                spring.prevLength = spring.length;
                _prevBottomOutDistance = _bottomOutDistance;
            }
            
            // Calculate spring velocity even when in air
            spring.velocity = (spring.length - spring.prevLength) / _fixedDeltaTime;
            spring.compressionPercent = (spring.maxLength - spring.length) / spring.maxLength;
            spring.force = hasHit ? spring.maxForce * spring.forceCurve.Evaluate(spring.compressionPercent) : 0;

            if (spring.bottomedOut)
            {
                float overflowVelocity = (_bottomOutDistance - _prevBottomOutDistance) / _fixedDeltaTime;
                float wheelCountCoeff = 1f / (vehicleWheelCount > 0 ? vehicleWheelCount : 4);
                float bottomOutDistanceCoeff = _bottomOutDistance * _bottomOutDistance * 100f;
                float gravityCoeff = parentRigidbody.mass * -Physics.gravity.y;
                float bottomOutForce = gravityCoeff * bottomOutDistanceCoeff * wheelCountCoeff *
                                       spring.bottomOutForceCoefficient;
                bottomOutForce += gravityCoeff * -overflowVelocity * wheelCountCoeff *
                                  -spring.bottomOutForceCoefficient * 0.25f;
                parentRigidbody.AddForceAtPosition(bottomOutForce * _transformUp, _transformPosition);
            }
            else if (hasHit)
            {
                if (!hasHit)
                {
                    damper.force = 0;
                }

                if (spring.length <= spring.prevLength)
                {
                    damper.force = damper.bumpForce *
                                   damper.curve.Evaluate(
                                       spring.velocity < 0 ? -spring.velocity : spring.velocity);
                }
                else
                {
                    damper.force = -damper.reboundForce *
                                   damper.curve.Evaluate(
                                       spring.velocity < 0 ? -spring.velocity : spring.velocity);
                }
            }

            spring.prevLength = spring.length;

            suspensionForceMagnitude = hasHit
                ? Mathf.Clamp(spring.force + damper.force, 0.0f, Mathf.Infinity)
                : 0f;

            _prevBottomOutDistance = _bottomOutDistance;

            parentRigidbody.AddForceAtPosition(suspensionForceMagnitude * _raycastHitNormal, _transformPosition);
        }


        private void WheelUpdate()
        {
            wheel.worldPosition = _transformPosition - _transformUp * spring.length - wheel.inside * wheel.rimOffset;

            // Calculate camber based on spring travel
            wheel.camberAngle = Mathf.Lerp(wheel.camberAtTop, wheel.camberAtBottom, 1f - spring.compressionPercent);

            // Tire load calculated from spring and damper force for wheelcollider compatibility
            wheel.load = Mathf.Clamp(spring.force + damper.force, 0.0f, Mathf.Infinity);
            if (hasHit)
            {
                wheelHit.force = wheel.load;
            }

            // Calculate visual rotation angle between 0 and 2PI radians.
            wheel.rotationAngle =
                wheel.rotationAngle % 360.0f + wheel.angularVelocity * Mathf.Rad2Deg * _fixedDeltaTime;

            _axleRotation = Quaternion.AngleAxis(wheel.rotationAngle, _transformRight);

            // Set rotation   
            wheel.worldRotation = totalRotation * _axleRotation * _transformRotation;

            Vector3 position = wheel.worldPosition + wheel.Visual.transform.TransformVector(wheel.visualPositionOffset);
            Quaternion rotation = wheel.worldRotation * Quaternion.Euler(wheel.visualRotationOffset);
            cachedVisualTransform.SetPositionAndRotation(position, rotation);

            // Apply rotation and position to the non-rotationg objects if assigned
            if (!wheel.nonRotatingVisualIsNull)
            {
                Vector3 pos = wheel.right * wheel.nonRotatingPositionOffset.x
                              + wheel.up * wheel.nonRotatingPositionOffset.y
                              + wheel.forward * wheel.nonRotatingPositionOffset.z;
                wheel.NonRotatingVisual.transform.SetPositionAndRotation(wheel.worldPosition + pos,
                    totalRotation * _transformRotation);
            }

            // Apply rotation to rim collider 
            if (useRimCollider)
            {
                wheel.rimColliderGO.transform.SetPositionAndRotation(wheel.worldPosition,
                    steerQuaternion * camberQuaternion * _transformRotation);
            }
        }

        /// <summary>
        ///     Positions wheel to stick to the ground, does not calculate any forces at all.
        ///     Intended to be used with multiplayer for client vehicles.
        /// </summary>
        public void VisualUpdate()
        {
            spring.targetPoint = wheelHit.raycastHit.point - wheel.right * (wheel.rimOffset * (int) vehicleSide);
            spring.length = -cachedTransform.InverseTransformPoint(spring.targetPoint).y;
            spring.length = Mathf.Clamp(spring.length, 0, spring.maxLength);
            wheel.camberAngle = Mathf.Lerp(wheel.camberAtTop, wheel.camberAtBottom, spring.length / spring.maxLength);

            _prevMpPosition = wheel.worldPosition;
            wheel.worldPosition = _transformPosition - _transformUp * spring.length - wheel.inside * wheel.rimOffset;
            wheel.worldRotation = totalRotation * _transformRotation;

            Vector3 wheelVelocity = (wheel.worldPosition - _prevMpPosition) / _fixedDeltaTime;
            wheel.angularVelocity = transform.InverseTransformVector(wheelVelocity).z / wheel.radius;
            wheel.rotationAngle =
                wheel.rotationAngle % 360.0f + wheel.angularVelocity * Mathf.Rad2Deg * _fixedDeltaTime;
            steerQuaternion = Quaternion.AngleAxis(wheel.steerAngle, _transformUp);
            _axleRotation = Quaternion.AngleAxis(wheel.rotationAngle, _transformRight);
            wheel.worldRotation = steerQuaternion * _axleRotation * _transformRotation;

            // Apply rotation and position to visuals if assigned
            Vector3 position = wheel.worldPosition + wheel.Visual.transform.TransformVector(wheel.visualPositionOffset);
            wheel.Visual.transform.SetPositionAndRotation(position,
                wheel.worldRotation);

            // Apply rotation and position to the non-rotationg objects if assigned
            if (!wheel.nonRotatingVisualIsNull)
            {
                Vector3 pos = wheel.right * wheel.nonRotatingPositionOffset.x
                              + wheel.up * wheel.nonRotatingPositionOffset.y
                              + wheel.forward * wheel.nonRotatingPositionOffset.z;
                wheel.NonRotatingVisual.transform.SetPositionAndRotation(wheel.worldPosition + pos,
                    totalRotation * _transformRotation);
            }
        }

        /// <summary>
        ///     Lateral and longitudinal slip and force calculations.
        /// </summary>
        private void FrictionUpdate()
        {
            _contactVelocity = parentRigidbody.GetPointVelocity(wheel.worldPosition - wheel.up * wheel.radius);

            // Account for moving surfaces
            if (wheelHit.raycastHit.rigidbody)
            {
                _contactVelocity -= wheelHit.raycastHit.rigidbody.GetPointVelocity(_wheelHitPoint);
            }

            if (hasHit)
            {
                forwardFriction.speed = Vector3.Dot(_contactVelocity, wheelHit.forwardDir);
                sideFriction.speed = Vector3.Dot(_contactVelocity, wheelHit.sidewaysDir);
            }
            else
            {
                forwardFriction.speed = sideFriction.speed = 0;
            }

            float loadCoefficient = CalculateLoadCoefficient();
            float contactVelocityMagnitude = _contactVelocity.magnitude;

            if (!useExternalLatSlipCalculation)
            {
                sideFriction.slip = 0f;
                sideFriction.force = 0f;

                Friction.CalculateLateralSlip(_fixedDeltaTime, contactVelocityMagnitude, wheel.angularVelocity,
                    loadCoefficient,
                    forwardFriction.speed, ref activeFrictionPreset, ref sideFriction, hasHit, out sideFriction.force);
            }

            if (!useExternalLongSlipCalculation)
            {
                forwardFriction.slip = 0f;
                forwardFriction.force = 0f;

                float outSurfaceTorque = 0;
                Friction.CalculateLongitudinalSlip(wheel.motorTorque, wheel.brakeTorque, dragTorque, wheel.radius, wheel.inertia,
                    _fixedDeltaTime, _fixedDeltaTime, loadCoefficient, activeFrictionPreset.BCDE.z, ref forwardFriction,
                    ref wheel.angularVelocity, ref outSurfaceTorque);
                forwardFriction.force = outSurfaceTorque / wheel.radius;
            }

            // Convert angular velocity to RPM
            wheel.RPM = wheel.angularVelocity * 9.55f;

            // Fill in WheelHit info for Unity wheelcollider compatibility
            if (hasHit)
            {
                wheelHit.forwardSlip = forwardFriction.slip;
                wheelHit.sidewaysSlip = sideFriction.slip;
            }


            Vector2 normalizedForce = new Vector2(forwardFriction.force, sideFriction.force);
            normalizedForce = Vector2.ClampMagnitude(normalizedForce, loadCoefficient);

            forwardFriction.force = normalizedForce.x;
            sideFriction.force = normalizedForce.y;
        }

        /// <summary>
        ///     Updates force values, calculates force vector and applies it to the rigidbody.
        /// </summary>
        private void UpdateForces()
        {
            float chassisTorque = 0;

            if (hasHit)
            {
                // Use alternate normal when encountering obstracles that have sharp edges in which case raycastHit.normal will alwyas point up.
                // Alternate normal cannot be used when on flat surface because of inaccuracies which cause vehicle to creep forward or in reverse.
                // Sharp edge detection is done via dot product of bot normals, if it differs it means that raycasHit.normal in not correct.

                // Cache most used values
                _wheelHitPoint = wheelHit.point;
                _raycastHitNormal = wheelHit.raycastHit.normal;

                // Hit direction
                _hitDir.x = wheel.worldPosition.x - _wheelHitPoint.x;
                _hitDir.y = wheel.worldPosition.y - _wheelHitPoint.y;
                _hitDir.z = wheel.worldPosition.z - _wheelHitPoint.z;

                // Alternate normal
                float distance = Mathf.Sqrt(_hitDir.x * _hitDir.x + _hitDir.y * _hitDir.y + _hitDir.z * _hitDir.z);
                _alternateForwardNormal.x = _hitDir.x / distance;
                _alternateForwardNormal.y = _hitDir.y / distance;
                _alternateForwardNormal.z = _hitDir.z / distance;
                _alternateForwardNormal = _alternateForwardNormal.normalized;

                if (Vector3.Dot(_raycastHitNormal, _transformUp) > 0.1f)
                {
                    // Obstracle force
                    float obstacleForceMag = 0f;

                    float absSpeed = forwardFriction.speed < 0 ? -forwardFriction.speed : forwardFriction.speed;
                    if (absSpeed < 8f)
                    {
                        float dot = Vector3.Dot(wheelHit.normal, _alternateForwardNormal);
                        dot = dot < 0 ? -dot : dot;
                        obstacleForceMag = (1f - dot) * suspensionForceMagnitude *
                                           (wheelHit.angleForward < 0 ? 1f : -1f);
                    }

                    _surfaceForceVector.x = obstacleForceMag * wheel.forward.x +
                                            wheelHit.sidewaysDir.x * -sideFriction.force +
                                            wheelHit.forwardDir.x * forwardFriction.force;
                    _surfaceForceVector.y = obstacleForceMag * wheel.forward.y +
                                            wheelHit.sidewaysDir.y * -sideFriction.force +
                                            wheelHit.forwardDir.y * forwardFriction.force;
                    _surfaceForceVector.z = obstacleForceMag * wheel.forward.x +
                                            wheelHit.sidewaysDir.z * -sideFriction.force +
                                            wheelHit.forwardDir.z * forwardFriction.force;
                    parentRigidbody.AddForceAtPosition(_surfaceForceVector, _wheelHitPoint);

                    if (applyForceToOthers)
                    {
                        if (wheelHit.raycastHit.rigidbody)
                        {
                            Rigidbody hitRigidbody = wheelHit.raycastHit.rigidbody;
                            hitRigidbody.AddForceAtPosition(-_surfaceForceVector, _wheelHitPoint);
                            hitRigidbody.AddForceAtPosition(-suspensionForceMagnitude * _raycastHitNormal,
                                _wheelHitPoint);
                        }
                    }
                }

                // Add torque. Not entirely accurate as braking torque from engine that gets transferred from the wheels
                // is not counted.
                if (squat != 0 && forwardFriction.force > 0)
                {
                    chassisTorque = forwardFriction.force * wheel.radius * squat;
                }
            }

            if (hasBeenEnabledThisFrame)
            {
                wheel.prevAngularVelocity = wheel.angularVelocity;
            }
            
            // Add torque as a result of dW
            float dwTorque = (wheel.angularVelocity - wheel.prevAngularVelocity) * wheel.inertia / _fixedDeltaTime;
            chassisTorque += dwTorque;

            Vector3 torqueForce = _transformForward * (chassisTorque * 0.5f);
            parentRigidbody.AddForceAtPosition(-torqueForce, wheel.worldPosition + wheel.up);
            parentRigidbody.AddForceAtPosition(torqueForce, wheel.worldPosition - wheel.up);
        }


        private void OnDisable()
        {
            OnDestroy();
        }

        /// <summary>
        ///     Visual representation of the wheel and it's more important Vectors.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                _transformPosition = transform.position;
            }


            // Draw spring travel
            Gizmos.color = Color.green;
            Vector3 forwardOffset = transform.forward * 0.07f;
            Vector3 springOffset = transform.up * spring.maxLength;
            Gizmos.DrawLine(_transformPosition - forwardOffset, _transformPosition + forwardOffset);
            Gizmos.DrawLine(_transformPosition - springOffset - forwardOffset,
                _transformPosition - springOffset + forwardOffset);
            Gizmos.DrawLine(_transformPosition, _transformPosition - springOffset);

            Vector3 interpolatedPos = Vector3.zero;

            // Set dummy variables when in inspector.
            if (!Application.isPlaying)
            {
                if (!wheel.visualIsNull)
                {
                    wheel.worldPosition = wheel.Visual.transform.position;
                    wheel.up = wheel.Visual.transform.up;
                    wheel.forward = wheel.Visual.transform.forward;
                    wheel.right = wheel.Visual.transform.right;
                }
            }

            Gizmos.DrawSphere(wheel.worldPosition, 0.02f);

            // Draw wheel
            Gizmos.color = Color.green;
            DrawWheelGizmo(wheel.radius, wheel.width, wheel.worldPosition, wheel.up, wheel.forward, wheel.right);

            if (debug && Application.isPlaying)
            {
                // Draw wheel anchor normals
                Gizmos.color = Color.red;
                Gizmos.DrawRay(new Ray(wheel.worldPosition, wheel.up));
                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Ray(wheel.worldPosition, wheel.forward));
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(new Ray(wheel.worldPosition, wheel.right));
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(new Ray(wheel.worldPosition, wheel.inside));

                // Draw axle location
                if (spring.length < 0.01f)
                {
                    Gizmos.color = Color.red;
                }
                else if (spring.length > spring.maxLength - 0.01f)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.green;
                }

                if (hasHit)
                {
                    // Draw hit points
                    float weightSum = 0f;
                    float minWeight = Mathf.Infinity;
                    float maxWeight = 0f;

                    foreach (WheelHit hit in wheelHits)
                    {
                        weightSum += hit.weight;
                        if (hit.weight < minWeight)
                        {
                            minWeight = hit.weight;
                        }

                        if (hit.weight > maxWeight)
                        {
                            maxWeight = hit.weight;
                        }
                    }

                    foreach (WheelHit hit in wheelHits)
                    {
                        float t = (hit.weight - minWeight) / (maxWeight - minWeight);
                        Gizmos.color = Color.Lerp(Color.black, Color.white, t);
                        Gizmos.DrawSphere(hit.point, 0.04f);
                        Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
                        Gizmos.DrawLine(hit.point, hit.point + wheel.up * hit.distanceFromTire);
                    }

                    //Draw hit forward and sideways
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(wheelHit.point,
                        wheelHit.point + wheelHit.forwardDir * (forwardFriction.force * 0.001f));
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(wheelHit.point,
                        wheelHit.point - wheelHit.sidewaysDir * (sideFriction.force * 0.001f));

                    // Draw ground point
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(wheelHit.point, 0.04f);
                    Gizmos.DrawLine(wheelHit.point, wheelHit.point + wheelHit.normal * 1f);

                    Gizmos.color = Color.yellow;
                    Vector3 alternateNormal = (wheel.worldPosition - wheelHit.point).normalized;
                    Gizmos.DrawLine(wheelHit.point, wheelHit.point + alternateNormal * 1f);

                    // Spring travel point
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawCube(spring.targetPoint, new Vector3(0.1f, 0.1f, 0.04f));
                }
            }
        }


        /// <summary>
        ///     Returns Raycast info of the wheel's hit.
        ///     Always check if the function returns true before using hit info
        ///     as data will only be updated when wheel is hitting the ground (isGrounded == True).
        /// </summary>
        /// <param name="h">Standard Unity RaycastHit</param>
        /// <returns></returns>
        public bool GetGroundHit(out WheelHit hit)
        {
            hit = wheelHit;
            return hasHit;
        }

        /// <summary>
        ///     Returns the position and rotation of the wheel.
        /// </summary>
        public void GetWorldPose(out Vector3 pos, out Quaternion quat)
        {
            pos = wheel.worldPosition;
            quat = wheel.worldRotation;
        }

        /// <summary>
        ///     Sets linear camber betwen the two values.
        /// </summary>
        /// <param name="camberAtTop"></param>
        /// <param name="camberAtBottom"></param>
        public void SetCamber(float camberAtTop, float camberAtBottom)
        {
            wheel.camberAtTop = camberAtTop;
            wheel.camberAtBottom = camberAtBottom;
        }

        /// <summary>
        ///     Sets fixed camber.
        /// </summary>
        /// <param name="camber"></param>
        public void SetCamber(float camber)
        {
            wheel.camberAtTop = wheel.camberAtBottom = camber;
        }

        private void Reset()
        {
            SetDefaults();
        }

        /// <summary>
        ///     Sets default values if they have not already been set.
        ///     Gets called each time Reset() is called in editor - such as adding the script to a GameObject.
        /// </summary>
        /// <param name="reset">Sets default values even if they have already been set.</param>
        /// <param name="findWheelVisuals">Should script attempt to find wheel visuals automatically by name and position?</param>
        public void SetDefaults(bool reset = false, bool findWheelVisuals = true)
        {
            // Objects
            if (parent == null || reset)
            {
                parent = FindParent();
                if (parent == null)
                {
                    Debug.LogWarning(
                        $"Parent Rigidbody of WheelController {name} could not be found. It will have to be assigned manually.");
                }
            }

            if (wheel == null || reset)
            {
                wheel = new Wheel();
            }

            if (spring == null || reset)
            {
                spring = new Spring();
            }

            if (damper == null || reset)
            {
                damper = new Damper();
            }

            if (forwardFriction == null || reset)
            {
                forwardFriction = new Friction();
            }

            if (sideFriction == null || reset)
            {
                sideFriction = new Friction();
            }

            // Friction preset
            if (activeFrictionPreset == null || reset)
            {
                activeFrictionPreset =
                    Resources.Load<FrictionPreset>("Wheel Controller 3D/Defaults/DefaultTireFrictionPreset");
            }

            // Curves
            if (springCurve == null || springCurve.keys.Length == 0 || reset)
            {
                springCurve = GenerateDefaultSpringCurve();
            }

            if (DamperCurve == null || DamperCurve.keys.Length == 0 || reset)
            {
                DamperCurve = GenerateDefaultDamperCurve();
            }

            if (loadGripCurve == null || loadGripCurve.keys.Length == 0 || reset)
            {
                loadGripCurve = GenerateDefaultLoadGripCurve();
            }

            // Side
            if (vehicleSide == Side.Auto && parent != null || reset)
            {
                vehicleSide = DetermineSide(transform.position, parent.transform);
            }

            // Attempt to find wheel visuals
            if (findWheelVisuals && wheel.Visual == null && parent != null)
            {
                Transform thisTransform = transform;
                Transform[] children = parent.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    Vector3 p1 = thisTransform.position;
                    Vector3 p2 = child.position;
                    float x = p2.x - p1.x;
                    float z = p2.z - p1.z;
                    float distance = Mathf.Sqrt(x * x + z * z);

                    if (distance < 0.2f)
                    {
                        string lowerName = child.name.ToLower();
                        if ((lowerName.Contains("wheel") || lowerName.Contains("whl")) &&
                            child.GetComponent<WheelController>() == null)
                        {
                            wheel.Visual = child.gameObject;
                        }
                    }

                    if (wheel.Visual)
                    {
                        Debug.LogWarning(
                            $"WheelController {name}: Could not auto-find wheel visual. Make sure to assign wheel model to the 'Visual' field of WheelController.");
                    }
                }
            }

            // Assign layer mask
            if (autoSetupLayerMask && parent != null)
            {
                SetupLayerMask();
            }
        }

        /// <summary>
        ///     Runs one physics update of the wheel.
        /// </summary>
        public void Step()
        {
            if (!_initialized)
            {
                Initialize();
            }

            bool initAutoSyncTransforms = Physics.autoSyncTransforms;
            Physics.autoSyncTransforms = false;

            _scale = cachedTransform.lossyScale;
            _yScale = _scale.y;

            // Check if wheel dimensions have changed and if so update relevant data
            if (wheel.radius != _prevRadius || wheel.width != _prevWidth)
            {
                wheel.Initialize(this);
                InitializeScanParams();
            }

            _prevRadius = wheel.radius;
            _prevWidth = wheel.width;

            UpdateCachedValues();
            HitUpdate();

            if (visualOnlyUpdate)
            {
                CalculateWheelDirectionsAndRotations();
                VisualUpdate();
            }
            else
            {
                if (!parentRigidbody.IsSleeping())
                {
                    SuspensionUpdate();
                    CalculateWheelDirectionsAndRotations();
                    WheelUpdate();
                    FrictionUpdate();
                    UpdateForces();
                }
            }

            wheel.prevAngularVelocity = wheel.angularVelocity;

            Physics.autoSyncTransforms = initAutoSyncTransforms;
        }


        private void OnDestroy()
        {
            try
            {
                _raycastCommands.Dispose();
                _raycastHits.Dispose();
            }
            catch
            {
            }
        }

        private void GenerateRaycastArraysIfNeeded(int size)
        {
            if (_raycastCommands == null || !_raycastCommands.IsCreated)
            {
                _raycastCommands = new NativeArray<RaycastCommand>(size, Allocator.Persistent);
                _raycastCommandsArray = new RaycastCommand[size];
            }

            if (_raycastHits == null || !_raycastHits.IsCreated)
            {
                _raycastHits = new NativeArray<RaycastHit>(size, Allocator.Persistent);
                _raycastHitsArray = new RaycastHit[size];
            }
        }


        private void CalculateWheelDirectionsAndRotations()
        {
            steerQuaternion = Quaternion.AngleAxis(wheel.steerAngle, _transformUp);
            camberQuaternion = Quaternion.AngleAxis(-(int) vehicleSide * wheel.camberAngle, _transformForward);
            totalRotation = steerQuaternion * camberQuaternion;

            wheel.up = totalRotation * _transformUp;
            wheel.forward = totalRotation * _transformForward;
            wheel.right = totalRotation * _transformRight;
            wheel.inside = wheel.right * -(int) vehicleSide;
        }

        private void CalculateAverageWheelHit()
        {
            int count = 0;

            // Weighted average
            WheelHit wheelRay;
            float n = wheelHits.Length;
            float minWeight = Mathf.Infinity;
            float maxWeight = 0f;
            float weightSum = 0f;
            int validCount = 0;

            n = wheelHits.Length;

            _hitPointSum = Vector3.zero;
            _normalSum = Vector3.zero;
            _weight = 0;

            float longSum = 0;
            float latSum = 0;
            float angleSum = 0;
            float offsetSum = 0;
            validCount = 0;

            for (int i = 0; i < n; i++)
            {
                wheelRay = wheelHits[i];
                if (wheelRay.valid)
                {
                    _weight = wheel.radius - wheelRay.distanceFromTire;
                    _weight = _weight * _weight * _weight;

                    if (_weight < minWeight)
                    {
                        minWeight = _weight;
                    }
                    else if (_weight > maxWeight)
                    {
                        maxWeight = _weight;
                    }

                    weightSum += _weight;
                    validCount++;

                    _normal = wheelRay.raycastHit.normal;
                    _point = wheelRay.raycastHit.point;

                    _hitPointSum.x += _point.x * _weight;
                    _hitPointSum.y += _point.y * _weight;
                    _hitPointSum.z += _point.z * _weight;

                    _normalSum.x += _normal.x * _weight;
                    _normalSum.y += _normal.y * _weight;
                    _normalSum.z += _normal.z * _weight;

                    longSum += wheelRay.offset.y * _weight;
                    latSum += wheelRay.offset.x * _weight;
                    angleSum += wheelRay.angleForward * _weight;
                    offsetSum += wheelRay.curvatureOffset * _weight;

                    count++;
                }
            }

            if (validCount == 0 || _minDistRayIndex < 0)
            {
                hasHit = false;
                return;
            }

            wheelHit.raycastHit = wheelHits[_minDistRayIndex].raycastHit;
            wheelHit.raycastHit.point = _hitPointSum / weightSum;
            wheelHit.offset.y = longSum / weightSum;
            wheelHit.offset.x = latSum / weightSum;
            wheelHit.angleForward = angleSum / weightSum;
            wheelHit.raycastHit.normal = Vector3.Normalize(_normalSum / weightSum);
            wheelHit.curvatureOffset = offsetSum / weightSum;

            wheelHit.raycastHit.point += wheel.up * wheelHit.curvatureOffset;
            wheelHit.groundPoint = wheelHit.raycastHit.point - wheel.up * wheelHit.curvatureOffset;
        }

        public void ApplyDamage(float damage)
        {
            _damage = damage < 0 ? 0 : damage > 1 ? 1 : damage;
            wheel.visualRotationOffset.z = _damage * 10f;
        }

        public float CalculateLoadCoefficient()
        {
            return loadGripCurve.Evaluate(Mathf.Clamp01(wheel.load / maximumTireLoad)) * maximumTireGripForce;
        }

        private GameObject FindParent()
        {
            Transform t = transform;
            while (t != null)
            {
                if (t.GetComponent<Rigidbody>())
                {
                    return t.gameObject;
                }

                t = t.parent;
            }

            return null;
        }

        private AnimationCurve GenerateDefaultSpringCurve()
        {
            AnimationCurve ac = new AnimationCurve();
            ac.AddKey(0.0f, 0.0f);
            ac.AddKey(1.0f, 1.0f);
            return ac;
        }


        private AnimationCurve GenerateDefaultDamperCurve()
        {
            AnimationCurve ac = new AnimationCurve();
            ac.AddKey(0f, 0f);
            ac.AddKey(1f, 1f);
            return ac;
        }

        private AnimationCurve GenerateDefaultLoadGripCurve()
        {
            AnimationCurve ac = new AnimationCurve
            {
                keys = new[]
                {
                    new Keyframe(0f, 0f, 0, 1f),
                    new Keyframe(0.35f, 0.6f, 1f, 1f),
                    new Keyframe(1f, 1f)
                }
            };

            return ac;
        }

        /// <summary>
        ///     Average of multiple Vector3's
        /// </summary>
        private Vector3 Vector3Average(List<Vector3> vectors)
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3 v in vectors)
            {
                sum += v;
            }

            return sum / vectors.Count;
        }


        /// <summary>
        ///     Calculates an angle between two vectors in relation a normal.
        /// </summary>
        /// <param name="v1">First Vector.</param>
        /// <param name="v2">Second Vector.</param>
        /// <param name="n">Angle around this vector.</param>
        /// <returns>Angle in degrees.</returns>
        private float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        /// <summary>
        ///     Determines on what side of the vehicle a point is.
        /// </summary>
        /// <param name="pointPosition">Position of the point in question.</param>
        /// <param name="referenceTransform">Position of the reference transform.</param>
        /// <returns>Enum Side [Left,Right] (int)[-1,1]</returns>
        public Side DetermineSide(Vector3 pointPosition, Transform referenceTransform)
        {
            Vector3 relativePoint = referenceTransform.InverseTransformPoint(pointPosition);

            if (relativePoint.x < 0.0f)
            {
                return Side.Left;
            }

            return Side.Right;
        }

        /// <summary>
        ///     Determines if layer is in layermask.
        /// </summary>
        public static bool IsInLayerMask(int layer, LayerMask layermask)
        {
            return layermask == (layermask | (1 << layer));
        }

        private void SetupLayerMask()
        {
            if (parent == null)
            {
                Debug.LogError("Cannot set up layer mask for null parent.");
                return;
            }

            List<GameObject> colliderGOs = new List<GameObject>();
            GetVehicleColliders(parent.transform, ref colliderGOs);
            List<string> layers = new List<string>();
            int n = colliderGOs.Count;
            for (int i = 0; i < n; i++)
            {
                string layer = LayerMask.LayerToName(colliderGOs[i].layer);
                if (layers.All(l => l != layer))
                {
                    layers.Add(layer);
                }
            }

            layers.Add(LayerMask.LayerToName(2));
            layerMask = ~LayerMask.GetMask(layers.ToArray());
        }

        private void GetVehicleColliders(Transform parent, ref List<GameObject> colliderGOs)
        {
            colliderGOs = new List<GameObject>();
            foreach (Collider collider in parent.GetComponentsInChildren<Collider>())
            {
                if (collider.gameObject.layer == 0)
                {
                    collider.gameObject.layer = 2;
                }

                colliderGOs.Add(collider.gameObject);
            }
        }

        /// <summary>
        ///     Draw a wheel radius on both side of the wheel, interconected with lines perpendicular to wheel axle.
        /// </summary>
        private void DrawWheelGizmo(float radius, float width, Vector3 position, Vector3 up, Vector3 forward,
            Vector3 right)
        {
            float halfWidth = width / 2.0f;
            float theta = 0.0f;
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            Vector3 pos = position + up * y + forward * x;
            Vector3 newPos = pos;

            for (theta = 0.0f; theta <= Mathf.PI * 2; theta += Mathf.PI / 12.0f)
            {
                x = radius * Mathf.Cos(theta);
                y = radius * Mathf.Sin(theta);
                newPos = position + up * y + forward * x;

                // Left line
                Gizmos.DrawLine(pos - right * halfWidth, newPos - right * halfWidth);

                // Right line
                Gizmos.DrawLine(pos + right * halfWidth, newPos + right * halfWidth);

                // Center Line
                Gizmos.DrawLine(pos - right * halfWidth, pos + right * halfWidth);

                // Diagonal
                Gizmos.DrawLine(pos - right * halfWidth, newPos + right * halfWidth);

                pos = newPos;
            }
        }
    }
}