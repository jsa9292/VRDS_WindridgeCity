using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Effects
{
    /// <summary>
    ///     Main class for handling visual effects such as skidmarks, lights and exhausts.
    /// </summary>
    [Serializable]
    public class EffectManager : VehicleComponent
    {
        /// <summary>
        ///     All effects are placed in this list after initialization.
        /// </summary>
        [Tooltip("    All effects are placed in this list after initialization.")]
        public List<VehicleComponent> components = new List<VehicleComponent>();

        public ExhaustFlash exhaustFlash = new ExhaustFlash();
        public ExhaustSmoke exhaustSmoke = new ExhaustSmoke();

        [FormerlySerializedAs("lights")]
        public LightsMananger lightsManager = new LightsMananger();

        [FormerlySerializedAs("skidmarks")]
        public SkidmarkManager skidmarkManager = new SkidmarkManager();

        public SurfaceParticleManager surfaceParticleManager = new SurfaceParticleManager();

        public override void Initialize()
        {
            initialized = true;
        }

        public override void Awake(VehicleController vc)
        {
            base.Awake(vc);

            GetComponents(ref components);
            foreach (VehicleComponent component in components)
            {
                component.Awake(vc);
            }
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

            foreach (VehicleComponent component in components)
            {
                component.Update();
            }
        }

        public override void OnDrawGizmosSelected(VehicleController vc)
        {
            base.OnDrawGizmosSelected(vc);

            if (components == null || components.Count == 0)
            {
                GetComponents(ref components);
            }

            foreach (VehicleComponent component in components)
            {
                component.OnDrawGizmosSelected(vc);
            }
        }

        public override void CheckState(int lodIndex)
        {
            base.CheckState(lodIndex);

            foreach (VehicleComponent component in components)
            {
                component.CheckState(lodIndex);
            }
        }

        public override void SetDefaults(VehicleController vc)
        {
            base.SetDefaults(vc);

            GetComponents(ref components);
            foreach (VehicleComponent component in components)
            {
                component.SetDefaults(vc);
            }
        }

        /// <summary>
        ///     Returns lists of all effects.
        /// </summary>
        private void GetComponents(ref List<VehicleComponent> components)
        {
            components = new List<VehicleComponent>
            {
                exhaustFlash,
                exhaustSmoke,
                lightsManager,
                skidmarkManager,
                surfaceParticleManager
            };
        }
    }
}