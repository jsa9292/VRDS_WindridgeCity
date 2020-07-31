using System;
using System.Linq;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Powertrain;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.VehiclePhysics2.Modules.TCS
{
    /// <summary>
    ///     Traction Control System (TCS) module. Reduces engine throttle when excessive slip is present.
    /// </summary>
    [Serializable]
    public class TCSModule : VehicleModule
    {
        public bool active;

        /// <summary>
        ///     Speed under which TCS will not work.
        /// </summary>
        [Tooltip("    Speed under which TCS will not work.")]
        public float lowerSpeedThreshold = 2f;

        /// <summary>
        ///     Longitudinal slip threshold at which TCS will activate.
        /// </summary>
        [Range(0f, 1f)]
        [ShowInSettings("Slip Threshold", 0f, 1f, 0.05f)]
        [Tooltip("    Longitudinal slip threshold at which TCS will activate.")]
        public float slipThreshold = 0.1f;

        /// <summary>
        ///     Called each frame while TCS is active.
        /// </summary>
        [Tooltip("    Called each frame while TCS is active.")]
        public UnityEvent TCSActive = new UnityEvent();

        public override void Initialize()
        {
            initialized = true;
        }

        public override void FixedUpdate()
        {
        }

        public override void Update()
        {
        }

        public override void Enable()
        {
            base.Enable();

            if (vc != null)
            {
                bool all = true;
                foreach (EngineComponent.PowerModifier p in vc.powertrain.engine.powerModifiers)
                {
                    if (p == TCSPowerLimiter)
                    {
                        all = false;
                        break;
                    }
                }

                if (all)
                {
                    vc.powertrain.engine.powerModifiers.Add(TCSPowerLimiter);
                }
            }
        }

        public override void Disable()
        {
            base.Disable();
            active = false;

            if (vc != null)
            {
                vc.powertrain.engine.powerModifiers.RemoveAll(p => p == TCSPowerLimiter);
            }
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.DrivingAssists;
        }


        public float TCSPowerLimiter()
        {
            active = false;

            if (!Active)
            {
                return 1f;
            }

            if (vc.Speed > lowerSpeedThreshold)
            {
                foreach (WheelComponent wheel in vc.Wheels)
                {
                    if (!wheel.IsGrounded || vc.powertrain.transmission.IsShifting)
                    {
                        continue;
                    }

                    float longSlip = wheel.LongitudinalSlip;
                    if (longSlip < 0 && longSlip < -slipThreshold)
                    {
                        active = true;
                        TCSActive.Invoke();
                        return 0f;
                    }
                }
            }

            return 1f;
        }
    }
}