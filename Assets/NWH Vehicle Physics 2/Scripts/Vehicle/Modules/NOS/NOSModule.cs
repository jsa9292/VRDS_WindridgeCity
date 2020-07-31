using System;
using System.Linq;
using NWH.VehiclePhysics2.Demo;
using NWH.VehiclePhysics2.Modules.NOS;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
    /// <summary>
    ///     NOS (Nitrous Oxide System) module.
    /// </summary>
    [Serializable]
    public class NOSModule : VehicleModule
    {
        /// <summary>
        ///     Is NOS currently active? Not to be confused with enabled.
        /// </summary>
        [Tooltip("    Is NOS currently active? Not to be confused with enabled.")]
        public bool active;

        /// <summary>
        ///     Capacity of NOS bottle.
        /// </summary>
        [ShowInSettings("Capacity", 0f, 5f, 0.5f)]
        [Tooltip("    Capacity of NOS bottle.")]
        public float capacity = 2f;

        /// <summary>
        ///     Current charge of NOS bottle.
        /// </summary>
        [ShowInSettings("Charge", 0f, 5f, 0.5f)]
        [Tooltip("    Current charge of NOS bottle.")]
        public float charge = 2f;

        /// <summary>
        ///     Can NOS be used while in reverse?
        /// </summary>
        [Tooltip("    Can NOS be used while in reverse?")]
        public bool disableInReverse = true;

        /// <summary>
        ///     Can NOS be used while there is no throttle input / engine is idling?
        /// </summary>
        [Tooltip("    Can NOS be used while there is no throttle input / engine is idling?")]
        public bool disableOffThrottle = true;

        /// <summary>
        ///     Makes engine sound louder while NOS is active.
        ///     Volume range of the engine running sound component will get multiplied by this value.
        /// </summary>
        [Range(1, 3)]
        [Tooltip(
            "Makes engine sound louder while NOS is active.\r\nVolume range of the engine running sound component will get multiplied by this value.")]
        public float engineVolumeCoefficient = 1.5f;

        /// <summary>
        ///     Value that will be used as base intensity of Exhaust Smoke effect while NOS is active.
        /// </summary>
        [Range(1, 3)]
        [Tooltip("    Value that will be used as base intensity of Exhaust Smoke effect while NOS is active.")]
        public float exhaustEmissionCoefficient = 2f;

        /// <summary>
        ///     Maximum flow of NOS in kg/s.
        /// </summary>
        [ShowInSettings("Flow", 0f, 2f)]
        [Tooltip("    Maximum flow of NOS in kg/s.")]
        public float flow = 0.1f;

        /// <summary>
        ///     Power of the engine will be multiplied by this value when NOS is active to get the final engine power.
        /// </summary>
        [Range(1, 5)]
        [ShowInSettings("Power Coeff.", 1f, 4f, 0.5f)]
        [Tooltip(
            "Power of the engine will be multiplied by this value when NOS is active to get the final engine power.")]
        public float powerCoefficient = 2f;

        [SerializeField]
        public NOSSoundComponent soundComponent = new NOSSoundComponent();
        
        private float initialEngineVolumeRange;
        private bool wasActive;

        // TODO - make new exhaust code work with NOS
        
        public override void Initialize()
        {
            soundComponent.nosModule = this;
            soundComponent.Awake(vc);
            soundComponent.container = vc.soundManager.engineSourceGO;
            soundComponent.audioMixerGroup = vc.soundManager.engineMixerGroup;
            soundComponent.Initialize();
            vc.soundManager.components.Add(soundComponent);
            
            initialEngineVolumeRange = vc.soundManager.engineRunningComponent.volumeRange;
            wasActive = active;

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

            wasActive = active;
            active = vc.input.states.boost;

            if (!active && wasActive)
            {
                vc.soundManager.engineRunningComponent.volumeRange = initialEngineVolumeRange;
            }
        }

        public override void Enable()
        {
            base.Enable();

            if (vc != null)
            {
                if (vc.powertrain.engine.powerModifiers.All(p => p != NOSPowerModifier))
                {
                    vc.powertrain.engine.powerModifiers.Add(NOSPowerModifier);
                }
            }
        }

        public override void Disable()
        {
            base.Disable();
            active = false;

            if (vc != null)
            {
                vc.powertrain.engine.powerModifiers.RemoveAll(p => p == NOSPowerModifier);
            }
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.Powertrain;
        }


        public float NOSPowerModifier()
        {
            if (!active || !Active || vc.powertrain.transmission.Ratio <= 0 && disableInReverse
                || vc.powertrain.engine.throttlePosition < 0.1f && disableOffThrottle)
            {
                active = false;
                return 1f;
            }

            charge -= flow * vc.fixedDeltaTime;
            charge = charge < 0 ? 0 : charge > capacity ? capacity : charge;

            if (charge <= 0)
            {
                return 1f;
            }

            if (vc.effectsManager.exhaustFlash.Active)
            {
                vc.effectsManager.exhaustFlash.flash = true;
            }

            if (vc.soundManager.Active && vc.soundManager.engineRunningComponent.Active)
            {
                vc.soundManager.engineRunningComponent.volumeRange = initialEngineVolumeRange * engineVolumeCoefficient;
            }

            return powerCoefficient;
        }
    }
}