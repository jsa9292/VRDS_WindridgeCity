using System;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.GroundDetection;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using NWH.WheelController3D;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NWH.VehiclePhysics2.Powertrain
{
    [Serializable]
    public class WheelComponent : PowertrainComponent
    {
        // Cached values
        [ShowInTelemetry]
        [Tooltip("Cached values")]
        public int surfaceMapIndex = -1;

        public SurfacePreset surfacePreset;
        public VehicleController vc;
        public WheelController wheelController;

        [NonSerialized]
        public WheelGroup wheelGroup;

        public WheelGroupSelector wheelGroupSelector = new WheelGroupSelector();
        private bool _awake = true;
        private float _BCDEz;
        private float _brakeTorque;
        private float _fixedDeltaTime;
        private float _linearVelocity;
        private float _loadCoefficient;
        private float _outputTorque;
        private float _radius;
        private bool _singleRayByDefault;
        private float _slip;
        private float _verticalLoad = 5000f;

        /// <summary>
        ///     Torque in Nm used to slow down the wheel.
        /// </summary>
        public float BrakeTorque
        {
            get { return wheelController.brakeTorque; }
            set { wheelController.brakeTorque = value; }
        }

        /// <summary>
        ///     GameObject cointaining WheelController component.
        /// </summary>
        public GameObject ControllerGO
        {
            get { return wheelController.gameObject; }
        }

        /// <summary>
        ///     Transform to which the wheel controller is attached.
        /// </summary>
        public Transform ControllerTransform
        {
            get { return wheelController.cachedTransform; }
        }

        /// <summary>
        ///     Damage that the wheel has suffered so far.
        /// </summary>
        public float Damage
        {
            get { return wheelController.Damage; }
            set { wheelController.Damage = value; }
        }

        /// <summary>
        ///     Random steer direction of a damaged wheel. Depending on the amount of the damage vehicle has received this
        ///     value will be multiplied by the steer angle making the wheel gradually point more and more in a random direction
        ///     drastically worsening the handling.
        /// </summary>
        public float DamageSteerDirection { get; private set; }

        /// <summary>
        ///     True if lateral slip is larger than side slip threshold.
        /// </summary>
        public bool HasLateralSlip
        {
            get { return NormalizedLateralSlip > vc.lateralSlipThreshold; }
        }

        /// <summary>
        ///     True if longitudinal slip is larger than longitudinal slip threshold.
        /// </summary>
        public bool HasLongitudinalSlip
        {
            get { return NormalizedLongitudinalSlip > vc.longitudinalSlipThreshold; }
        }

        /// <summary>
        ///     True if wheel is touching any object.
        /// </summary>
        public bool IsGrounded
        {
            get { return wheelController.isGrounded; }
        }

        public float LateralSlip
        {
            get { return wheelController.sideFriction.slip; }
        }

        public float LinearVelocity
        {
            get { return _linearVelocity; }
            set { _linearVelocity = value; }
        }

        public float LongitudinalSlip
        {
            get { return wheelController.forwardFriction.slip; }
        }

        /// <summary>
        ///     Torque in Nm used to accelerate the wheel.
        /// </summary>
        public float MotorTorque
        {
            get { return wheelController.motorTorque; }
            set { wheelController.motorTorque = value; }
        }

        /// <summary>
        ///     Lateral slip of the wheel.
        /// </summary>
        public float NormalizedLateralSlip
        {
            get
            {
                float value = wheelController.sideFriction.slip < 0
                    ? -wheelController.sideFriction.slip
                    : wheelController.sideFriction.slip;
                return value < 0 ? 0 : value > 1 ? 1 : value;
            }
        }

        /// <summary>
        ///     Longitudinal slip percentage where 1 represents slip equal to forward slip threshold.
        /// </summary>
        public float NormalizedLongitudinalSlip
        {
            get
            {
                float value = wheelController.forwardFriction.slip < 0
                    ? -wheelController.forwardFriction.slip
                    : wheelController.forwardFriction.slip;
                return value < 0 ? 0 : value > 1 ? 1 : value;
            }
        }

        public float OutputTorque
        {
            get { return _outputTorque; }
            set { _outputTorque = value; }
        }

        /// <summary>
        ///     Wheel radius.
        /// </summary>
        public float Radius
        {
            get { return wheelController.radius; }
        }

        public float Slip
        {
            get { return _slip; }
            set { _slip = value; }
        }

        /// <summary>
        ///     Distance from top to bottom of spring travel.
        /// </summary>
        public float SpringTravel
        {
            get { return wheelController.springCompression * wheelController.springLength; }
        }

        /// <summary>
        ///     Steer angle of the wheel in degrees.
        /// </summary>
        public float SteerAngle
        {
            get { return wheelController.steerAngle; }
            set { wheelController.steerAngle = value; }
        }

        public float VerticalLoad
        {
            get { return _verticalLoad; }
            set { _verticalLoad = value; }
        }

        /// <summary>
        ///     Wheel width.
        /// </summary>
        public float Width
        {
            get { return wheelController.width; }
        }

        public override void Initialize()
        {
            inertia = wheelController.wheel.inertia;
            DamageSteerDirection = Random.Range(-1f, 1f);
            _singleRayByDefault = wheelController.singleRay;
        }

        public void PostUpdate()
        {
            _slip = _slip < -1f ? -1f : _slip > 1f ? 1f : _slip;
            wheelController.motorTorque = _awake ? _outputTorque : 0f;
            wheelController.forwardFriction.force = _awake ? _outputTorque / wheelController.wheel.radius : 0f;
            wheelController.Step();
        }

        public void PreUpdate()
        {
            _outputTorque = 0f;
            wheelController.useExternalUpdate = true;
            wheelController.useExternalLongSlipCalculation = !_inputIsNull;
            wheelController.useExternalLatSlipCalculation = false;

            if (surfacePreset != null && surfacePreset.frictionPreset != null)
            {
                wheelController.activeFrictionPreset = surfacePreset.frictionPreset;
                _BCDEz = surfacePreset.frictionPreset.BCDE.z;
            }
            else
            {
                _BCDEz = 1f;
            }

            angularVelocity = wheelController.angularVelocity;
            LinearVelocity = wheelController.forwardFriction.speed;
            VerticalLoad = wheelController.wheel.load;
            _brakeTorque = BrakeTorque;
            _radius = Radius;
            _fixedDeltaTime = vc.fixedDeltaTime;
            _loadCoefficient = wheelController.CalculateLoadCoefficient();
        }

        /// <summary>
        ///     Adds brake torque to the wheel on top of the existing torque. Value is clamped to max brake torque.
        /// </summary>
        /// <param name="torque">Torque in Nm that will be applied to the wheel to slow it down.</param>
        /// <param name="registerAsBraking">If true brakes.IsBraking flag will be set. This triggers brake lights.</param>
        public void AddBrakeTorque(float torque, bool registerAsBraking = true)
        {
            if (wheelGroup != null)
            {
                torque *= wheelGroup.brakeCoefficient;
            }

            if (torque < 0)
            {
                wheelController.brakeTorque += 0f;
            }
            else
            {
                wheelController.brakeTorque += torque;
            }

            if (wheelController.brakeTorque > vc.brakes.maxTorque)
            {
                wheelController.brakeTorque = vc.brakes.maxTorque;
            }

            if (wheelController.brakeTorque < 0)
            {
                wheelController.brakeTorque = 0;
            }

            vc.brakes.IsBraking = registerAsBraking;
        }

        public override void OnPreSolve()
        {
            base.OnPreSolve();

            _outputTorque = 0;
        }

        public override float QueryAngularVelocity(float inputAngularVelocity, float dt)
        {
#if NVP_DEBUG_PT
            if (Powertrain.DEBUG) Debug.Log($"{name} (QueryAngularVelocity)\tReturn W = {angularVelocity}");
#endif
            return angularVelocity;
        }


        public override float QueryInertia()
        {
#if NVP_DEBUG_PT
            if (Powertrain.DEBUG) Debug.Log($"{name} (QueryInertia)\tReturn I = {inertia}");
#endif

            return inertia;
        }

        /// <summary>
        ///     Sets brake torque to the provided value. Use 0 to remove any braking.
        /// </summary>
        public void ResetBrakes(float value)
        {
            wheelController.brakeTorque = value < 0 ? -value : value;
        }

        public override float SendTorque(float torque, float inertiaSum, float dt)
        {
            float outTorque = 0;
            float returnTorque = Friction.CalculateLongitudinalSlip(torque, _brakeTorque, wheelController.dragTorque, _radius, inertia,
                dt, _fixedDeltaTime, _loadCoefficient, _BCDEz,
                ref wheelController.forwardFriction, ref wheelController.wheel.angularVelocity, ref outTorque);
            angularVelocity = wheelController.wheel.angularVelocity;
            _outputTorque += outTorque;
            return returnTorque;
        }

        /// <summary>
        ///     Adds brake torque as a percentage in range from 0 to 1.
        /// </summary>
        public void SetBrakeIntensity(float percent)
        {
            float absPercent = percent < 0 ? -percent : percent;
            absPercent = absPercent < 0 ? 0 : absPercent > 1 ? 1 : absPercent;
            AddBrakeTorque(vc.brakes.maxTorque * absPercent);
        }

        public void SetWheelGroup(int wheelGroupIndex)
        {
            wheelGroupSelector.index = wheelGroupIndex;
        }

        /// <summary>
        ///     Turns on single ray mode to prevent unnecessary raycasting for inactive wheels / vehicles.
        /// </summary>
        public override void OnDisable()
        {
            if (wheelController != null && vc.activeLOD != null && vc.activeLOD.singleRayGroundDetection)
            {
                wheelController.singleRay = true;
                wheelController.useExternalUpdate = false;
                wheelController.useExternalLatSlipCalculation = false;
                wheelController.useExternalLongSlipCalculation = false;
            }

            wheelController.motorTorque = 0;

            _awake = false;
        }

        public override void Validate(VehicleController vc)
        {
            base.Validate(vc);

            if (wheelController == null)
            {
                Debug.LogWarning("WheelController not set.");
                return;
            }

            // Check if wheel actually belongs to this vehicle
            if (wheelController.parent.GetInstanceID() != vc.gameObject.GetInstanceID())
            {
                Debug.LogError(
                    $"Wheel {wheelController.name} on vehicle {vc.name} belongs to {wheelController.parent.name}." +
                    " Make sure that you reassign the wheels when copying the script from one vehicle to another.");
            }

            Debug.Assert(wheelController.Visual != null,
                $"{wheelController.name}: Visual is null. Assign the wheel model to the Visual field of WheelController.");
            Debug.Assert(wheelController.radius > 0, $"{wheelController.name}: Wheel radius must be positive.");
            Debug.Assert(wheelController.width > 0, $"{wheelController.name}: Wheel width must be positive.");
            Debug.Assert(wheelController.mass > 0, $"{wheelController.name}: Wheel mass must be positive.");
            Debug.Assert(wheelController.parent != null,
                $"{wheelController.name}: Parent of WheelController {name} not assigned");
            Debug.Assert(vc.gameObject.GetInstanceID() == wheelController.parent.gameObject.GetInstanceID(),
                $"{wheelController.name}: Parent object and the" +
                " vehicle this wheel is attached to are not the same GameObject." +
                " This could happen if you copied over VehicleController from one vehicle to another without reassigning the wheels.");
            Debug.Assert(wheelController.spring.maxForce > 0, $"{wheelController.name}: Spring force must be positive");
            Debug.Assert(wheelController.spring.forceCurve.keys.Length > 1,
                $"{wheelController.name}: Spring curve not set up.");
            Debug.Assert(wheelController.spring.maxLength > Time.fixedDeltaTime * 10f,
                $"{name}: Spring length might be too short ({wheelController.spring.maxLength}). Recommended minimum length is Time.fixedDeltaTime * 10f ({Time.fixedDeltaTime * 10f} for current settings).");
            Debug.Assert(wheelController.damper.bumpForce > 0f,
                $"{wheelController.name}: Bump force must be positive.");
            Debug.Assert(wheelController.damper.reboundForce > 0f,
                $"{wheelController.name}: Rebound force must be positive.");
            Debug.Assert(wheelController.damper.curve.keys.Length > 1,
                $"{wheelController.name}: Damper curve not set up");
            Debug.Assert(wheelController.activeFrictionPreset != null,
                $"{wheelController.name}: Active friction preset not assigned (null).");
        }

        /// <summary>
        ///     Activates the wheel after it has been suspended by turning off single ray mode. If the wheel is
        ///     in single ray mode by default it will be left on.
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (!_singleRayByDefault)
            {
                wheelController.singleRay = false;
            }

            _awake = true;
        }
    }
}