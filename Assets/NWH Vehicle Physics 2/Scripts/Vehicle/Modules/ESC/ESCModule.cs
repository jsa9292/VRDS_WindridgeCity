using System;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.ESC
{
    /// <summary>
    ///     Electronic Stability Control (ESC) module.
    ///     Applies braking on individual wheels to try and stabilize the vehicle when the vehicle velocity and vehicle
    ///     direction do not match.
    /// </summary>
    [Serializable]
    public class ESCModule : VehicleModule
    {
        /// <summary>
        ///     Intensity of stability control.
        /// </summary>
        [Range(0, 1)]
        [ShowInSettings("ESC Intensity", 0f, 1f, 0.05f)]
        [Tooltip("    Intensity of stability control.")]
        public float intensity = 0.4f;

        /// <summary>
        ///     ESC will not work below this speed.
        ///     Setting this to a too low value might cause vehicle to be hard to steer at very low speeds.
        /// </summary>
        [Tooltip(
            "ESC will not work below this speed.\r\nSetting this to a too low value might cause vehicle to be hard to steer at very low speeds.")]
        public float lowerSpeedThreshold = 4f;

        public override void Initialize()
        {
            initialized = true;
        }

        public override void FixedUpdate()
        {
        }


        public override void Update()
        {
            if (!Active)
            {
                return;
            }

            // Prevent ESC from working in reverse and at low speeds
            if (vc.ForwardVelocity < lowerSpeedThreshold)
            {
                return;
            }

            float angle = Vector3.SignedAngle(vc.vehicleRigidbody.velocity, vc.vehicleTransform.forward,
                vc.vehicleTransform.up);
            angle -= vc.steering.Angle * 0.5f;
            float absAngle = angle < 0 ? -angle : angle;

            if (vc.powertrain.engine.revLimiterActive || absAngle < 2f)
            {
                return;
            }

            foreach (WheelComponent wheel in vc.Wheels)
            {
                if (!wheel.IsGrounded)
                {
                    continue;
                }

                float additionalBrakeTorque = -angle * (int) wheel.wheelController.vehicleSide * 50f * intensity;
                wheel.AddBrakeTorque(additionalBrakeTorque, false);
            }
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.DrivingAssists;
        }
    }
}