using System;
using System.Collections.Generic;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Powertrain;
using NWH.VehiclePhysics2.Powertrain.Wheel;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     Assigns brake torque to individual wheels. Actual braking happens inside WheelController.
    /// </summary>
    [Serializable]
    public class Brakes : VehicleComponent
    {
        public delegate float BrakeTorqueModifier();

        /// <summary>
        ///     Set to true to use the air brake sound effect.
        /// </summary>
        [Tooltip("    Set to true to use the air brake sound effect.")]
        public bool airBrakes;

        public bool airBrakeSoundFlag;

        /// <summary>
        /// Should brakes be applied automatically when throttle is released?
        /// </summary>
        [UnityEngine.Tooltip("Should brakes be applied automatically when throttle is released?")]
        public bool brakeOffThrottle = false;

        /// <summary>
        /// Strength of off-throttle braking in percentage [0 to 1] of max braking torque.
        /// </summary>
        [Range(0, 1)] public float brakeOffThrottleStrength = 0.2f;

        /// <summary>
        ///     Should vehicle automatically brake when input direction is different from vehicle movement direction?
        /// </summary>
        [Tooltip(
            "Should vehicle automatically brake when input direction is different from vehicle movement direction?")]
        public bool brakeOnReverseDirection = true;

        /// <summary>
        ///     Collection of functions that modify the braking performance of the vehicle. Used for modules such as ABS where
        ///     brakes need to be overriden or their effect reduced/increase. Return 1 for neutral modifier while returning 0 will
        ///     disable the brakes completely. All brake torque modifiers will be multiplied in order to get the final brake torque
        ///     coefficient.
        /// </summary>
        [Tooltip(
            "Collection of functions that modify the braking performance of the vehicle. Used for modules such as ABS where brakes need to be overriden or their effect reduced/increase. Return 1 for neutral modifier while returning 0 will disable the brakes completely. All brake torque modifiers will be multiplied in order to get the final brake torque coefficient.")]
        public List<BrakeTorqueModifier> brakeTorqueModifiers = new List<BrakeTorqueModifier>();

        /// <summary>
        ///     Should brakes be applied when vehicle is asleep (IsAwake == false)?
        /// </summary>
        [Tooltip("    Should brakes be applied when vehicle is asleep (IsAwake == false)?")]
        public bool brakeWhileAsleep = true;

        /// <summary>
        ///     If true vehicle will break when in neutral and no throttle is applied.
        /// </summary>
        [Tooltip("    If true vehicle will break when in neutral and no throttle is applied.")]
        public bool brakeWhileIdle = true;

        /// <summary>
        ///     Max brake torque that can be applied to each wheel. To adjust braking on per-axle basis change brake coefficients
        ///     under Axle settings.
        /// </summary>
        [Tooltip("Max brake torque that can be applied to each wheel. " +
                 "To adjust braking on per-axle basis change brake coefficients under Axle settings")]
        [ShowInSettings("Brake Torque", 1000f, 10000f, 500f)]
        public float maxTorque = 5000f;

        /// <summary>
        ///     If the vehicle is traveling in the opposite direction from user input auto braking will happen above/under this
        ///     velocity [m/s]
        /// </summary>
        [Tooltip(
            "If the vehicle is traveling in the opposite direction from user input, auto braking will happen above/under this velocity [m/s]. E.g." +
            "user is pressing W and the vehicle is going 2 m/s backwards. Vehicle will break until it slows down to under threshold velocity, shift into first and start to accelerate.")]
        [Range(0.05f, 2f)]
        public float reverseDirectionVelocityThreshold = 0.34f;

        /// <summary>
        ///     Time in seconds needed to reach full braking torque.
        /// </summary>
        [Range(0f, 5f)]
        [ShowInSettings("Brake Smoothing")]
        [Tooltip("    Time in seconds needed to reach full braking torque.")]
        public float smoothing;

        private float _intensity;
        private float _intensityVelocity;

        /// <summary>
        ///     Is the vehicle currently braking?
        /// </summary>
        [SerializeField]
        private bool _isBraking;


        /// <summary>
        ///     Returns true if vehicle is currently braking. Will return true if there is ANY brake torque applied to the wheels.
        /// </summary>
        public bool IsBraking
        {
            get { return _isBraking; }
            set { _isBraking = value; }
        }

        public override void Initialize()
        {
            initialized = true;
        }

        public override void FixedUpdate()
        {
            if (!Active)
            {
                return;
            }

            // Reset brakes for this frame
            foreach (WheelComponent wc in vc.Wheels)
            {
                wc.ResetBrakes(0);
            }

            float brakingIntensityModifier = SumBrakeTorqueModifiers();
            float verticalInput = vc.input.Vertical;
            float absVerticalInput = verticalInput < 0 ? -verticalInput : verticalInput;

            if (brakingIntensityModifier < 0.01f)
            {
                return;
            }

            // Everything after this point will set brake flag to active and thus register as braking.
            _isBraking = false;
            int currentGear = vc.powertrain.transmission.Gear;

            if (brakeOffThrottle && verticalInput < 0.05f)
            {
                foreach (WheelGroup axle in vc.WheelGroups)
                {
                    if (axle.handbrakeCoefficient > 0)
                    {
                        foreach (WheelComponent wheel in axle.Wheels)
                        {
                            wheel.SetBrakeIntensity(brakeOffThrottleStrength * axle.brakeCoefficient);
                        }
                    }
                }
            }
            
            // Handbrake
            if (vc.input.Handbrake > 0.02f && vc.IsAwake)
            {
                foreach (WheelGroup axle in vc.WheelGroups)
                {
                    if (axle.handbrakeCoefficient > 0)
                    {
                        foreach (WheelComponent wheel in axle.Wheels)
                        {
                            wheel.AddBrakeTorque(maxTorque * axle.handbrakeCoefficient * vc.input.Handbrake *
                                                 brakingIntensityModifier);
                        }
                    }
                }
            }

            // Brake while idle or asleep
            bool idleBrake = brakeWhileIdle && absVerticalInput < 0.01f && currentGear == 0 && vc.Speed < 0.3f;
            bool sleepBrake = brakeWhileAsleep && !vc.IsAwake;
            if (idleBrake || sleepBrake)
            {
                foreach (WheelGroup wheelGroup in vc.WheelGroups)
                foreach (WheelComponent wheel in wheelGroup.Wheels)
                {
                    wheel.SetBrakeIntensity(brakingIntensityModifier);
                }
            }

            // Brake on reverse direction
            if (brakeOnReverseDirection)
            {
                _intensity = Mathf.SmoothDamp(_intensity, absVerticalInput, ref _intensityVelocity, smoothing);

                if (!vc.powertrain.HasWheelSpin || !(vc.powertrain.engine.throttlePosition > 0.5f))
                {
                    if (vc.input.throttleType == Input.Input.ThrottleType.WForwardSReverse)
                    {
                        float forwardVelocity = vc.ForwardVelocity;
                        bool velocityMismatch =
                            forwardVelocity < -reverseDirectionVelocityThreshold && verticalInput > 0 ||
                            forwardVelocity > reverseDirectionVelocityThreshold && verticalInput < 0;
                        bool gearMismatch =
                            verticalInput > 0 && currentGear < 0 || verticalInput < 0 && currentGear > 0;
                        bool inputActive = verticalInput > 0.05f || verticalInput < -0.05f;

                        if (inputActive && (velocityMismatch || gearMismatch))
                        {
                            foreach (WheelGroup wheelGroup in vc.WheelGroups)
                            foreach (WheelComponent wheel in wheelGroup.Wheels)
                            {
                                wheel.SetBrakeIntensity(_intensity * brakingIntensityModifier *
                                                        wheelGroup.brakeCoefficient);
                            }
                        }
                        else
                        {
                            _intensity = 0;
                        }
                    }
                    else
                    {
                        if (vc.input.Vertical < 0)
                        {
                            foreach (WheelGroup wheelGroup in vc.WheelGroups)
                            foreach (WheelComponent wheel in wheelGroup.Wheels)
                            {
                                wheel.SetBrakeIntensity(_intensity * brakingIntensityModifier *
                                                        wheelGroup.brakeCoefficient);
                            }
                        }
                        else
                        {
                            _intensity = 0;
                        }
                    }
                }
            }

            // Air brakes pressure used for sound effects
            if (airBrakes)
            {
                //airBrakePressure += vc.fixedDeltaTime * 1f;
                //airBrakePressure = Mathf.Clamp(airBrakePressure, 0f, 3f);
            }
        }

        public override void Update()
        {
        }

        public override void Disable()
        {
            base.Disable();

            _isBraking = false;
        }

        private float SumBrakeTorqueModifiers()
        {
            if (brakeTorqueModifiers.Count == 0)
            {
                return 1f;
            }

            float coefficient = 1;
            int n = brakeTorqueModifiers.Count;
            for (int i = 0; i < n; i++)
            {
                coefficient *= brakeTorqueModifiers[i].Invoke();
            }

            return Mathf.Clamp(coefficient, 0f, Mathf.Infinity);
        }
    }
}