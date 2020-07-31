using System;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.FlipOver
{
    /// <summary>
    ///     Flip over module. Flips the vehicle over to be the right side up if needed.
    /// </summary>
    [Serializable]
    public class FlipOverModule : VehicleModule
    {
        /// <summary>
        ///     Minimum angle that the vehicle needs to be at for it to be detected as flipped over.
        /// </summary>
        [Tooltip("    Minimum angle that the vehicle needs to be at for it to be detected as flipped over.")]
        public float allowedAngle = 70f;

        public bool flipOverInput;

        /// <summary>
        ///     Is the vehicle flipped over?
        /// </summary>
        [Tooltip("    Is the vehicle flipped over?")]
        public bool flippedOver;

        /// <summary>
        ///     If enabled a prompt will be shown after the timeout, asking player to press the FlipOverModule button.
        /// </summary>
        [Tooltip(
            "If enabled a prompt will be shown after the timeout, asking player to press the FlipOverModule button.")]
        public bool manual;

        /// <summary>
        ///     Flip over detection will be disabled if velocity is above this value [m/s].
        /// </summary>
        [Tooltip("    Flip over detection will be disabled if velocity is above this value [m/s].")]
        public float maxDetectionSpeed = 1f;

        /// <summary>
        ///     Rotation speed of the vehicle while being flipped back.
        /// </summary>
        [Tooltip("    Rotation speed of the vehicle while being flipped back.")]
        public float rotationSpeed = 1f;

        /// <summary>
        ///     Time after detecting flip over after which vehicle will be flipped back.
        /// </summary>
        [Tooltip(
            "Time after detecting flip over after which vehicle will be flipped back or the manual button can be used.")]
        public float timeout = 3f;

        private bool _manualFlipoverInProgress;

        private float _xAngle;
        private float _zAngle;
        private float _timeAfterRecovery;
        private float _timeSinceFlip;
        private float _vehicleAngle;
        private bool _wasFlippedOver;

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

            _vehicleAngle = Vector3.Angle(vc.transform.up, -Physics.gravity.normalized);

            // Detect flipover
            int wheelsOnGround = 0;
            foreach (WheelComponent wheel in vc.Wheels)
            {
                if (wheel.IsGrounded)
                {
                    wheelsOnGround++;
                }
            }

            if (vc.Speed < maxDetectionSpeed && _vehicleAngle > allowedAngle && wheelsOnGround <= vc.Wheels.Count / 2f)
            {
                _timeSinceFlip += vc.fixedDeltaTime;

                if (_timeSinceFlip > timeout)
                {
                    flippedOver = true;
                }
            }
            else
            {
                _timeAfterRecovery += vc.fixedDeltaTime;

                if (_timeAfterRecovery > 1f || _vehicleAngle < 45f)
                {
                    flippedOver = false;
                    _timeSinceFlip = 0f;
                    _timeAfterRecovery = 0f;
                }
            }

            // Check for user input 
            if (manual)
            {
                try
                {
                    // Set manual flip over flag to true if vehicle is flipped over, otherwise ignore
                    if (UnityEngine.Input.GetButtonDown("FlipOver") && flippedOver)
                    {
                        flipOverInput = true;
                    }
                }
                catch
                {
                    Debug.LogError(
                        "Flip over is set to manual but 'FlipOverModule' input binding is not set. Either disable manual flip over or set 'FlipOverModule' binding.");
                }
            }

            // Return if flip over disabled.
            if (manual && flipOverInput)
            {
                _manualFlipoverInProgress = true;
                flipOverInput = false;
            }

            if (flippedOver && !manual || flippedOver && manual && _manualFlipoverInProgress)
            {
                if (_zAngle == 0 && _xAngle == 0)
                {
                    Vector3 eulerAngles = vc.vehicleTransform.eulerAngles;
                    _xAngle = eulerAngles.x * Time.smoothDeltaTime;
                    _zAngle = eulerAngles.z * Time.smoothDeltaTime;
                }
                
                vc.vehicleRigidbody.MoveRotation(vc.transform.rotation * Quaternion.Euler(_xAngle, 0, _zAngle));
            }
            else if (_wasFlippedOver && !flippedOver)
            {
                vc.vehicleRigidbody.constraints = RigidbodyConstraints.None;
                _manualFlipoverInProgress = false;
                _zAngle = _xAngle = 0;
            }

            _wasFlippedOver = flippedOver;
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.DrivingAssists;
        }
    }
}