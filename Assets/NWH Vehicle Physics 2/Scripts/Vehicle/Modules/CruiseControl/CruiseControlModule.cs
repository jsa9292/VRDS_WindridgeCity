using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.CruiseControl
{
    /// <summary>
    ///     Cruise Control implemented through a PID controller.
    ///     Does not work in reverse.
    ///     Can both accelerated and brake.
    /// </summary>
    [Serializable]
    public class CruiseControlModule : VehicleModule
    {
        /// <summary>
        ///     Derivative gain of PID controller.
        /// </summary>
        [Tooltip("    Derivative gain of PID controller.")]
        public float Kd = 0.1f;

        /// <summary>
        ///     Integral gain of PID controller.
        /// </summary>
        [Tooltip("    Integral gain of PID controller.")]
        public float Ki = 0.25f;

        /// <summary>
        ///     Proportional gain of PID controller.
        /// </summary>
        [Tooltip("    Proportional gain of PID controller.")]
        public float Kp = 0.5f;

        /// <summary>
        ///     Should the speed be set automatically when the module is enabled?
        /// </summary>
        [Tooltip("    Should the speed be set automatically when the module is enabled?")]
        public bool setTargetSpeedOnEnable;

        /// <summary>
        ///     The speed the vehicle will try to hold.
        /// </summary>
        [Tooltip("    The speed the vehicle will try to hold.")]
        public float targetSpeed;

        private float _e;
        private float _ed;
        private float _ei;
        private float _eprev;

        private float output;
        private float prevTargetSpeed;

        public override void Initialize()
        {
            initialized = true;
        }

        public override void FixedUpdate()
        {
        }


        public override void Update()
        {
            if (vc.input.states.cruiseControl)
            {
                ToggleState();
            }
            
            if (!Active)
            {
                return;
            }

            if (targetSpeed < 0.0001f)
            {
                return;
            }

            float speed = vc.Speed;
            float dt = vc.fixedDeltaTime;

            _eprev = _e;
            _e = targetSpeed - speed;
            if (_e > -0.5f && _e < 0.5f)
            {
                _ei = 0f;
            }

            if (prevTargetSpeed != targetSpeed)
            {
                _ei = 0f;
            }

            _ei += _e * dt;
            _ed = (_e - _eprev) / dt;
            float newOutput = _e * Kp + _ei * Ki + _ed * Kd;
            newOutput = newOutput < -1f ? -1f : newOutput > 1f ? 1f : newOutput;
            output = Mathf.Lerp(output, newOutput, vc.fixedDeltaTime * 5f);

            vc.input.Vertical = output;

            prevTargetSpeed = targetSpeed;
        }

        public override void Enable()
        {
            base.Enable();

            if (vc != null && setTargetSpeedOnEnable)
            {
                targetSpeed = vc.Speed;
            }
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.DrivingAssists;
        }
    }
}