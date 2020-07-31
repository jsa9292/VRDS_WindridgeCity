using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Rigging
{
    /// <summary>
    ///     Module used to animate rigged models by moving the axle/wheel bones.
    /// </summary>
    [Serializable]
    public class RiggingModule : VehicleModule
    {
        public List<Bone> bones = new List<Bone>();

        public override void Initialize()
        {
            initialized = true;
        }

        public override void Awake(VehicleController vc)
        {
            base.Awake(vc);

            foreach (Bone bone in bones)
            {
                bone.Initialize();
            }
        }

        public override void FixedUpdate()
        {
        }


        public override void Update()
        {
            Vector3 forward = vc.vehicleTransform.forward;
            Vector3 up = vc.vehicleTransform.up;
            foreach (Bone bone in bones)
            {
                bone.Update(forward, up);
            }
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.Animation;
        }
    }
}