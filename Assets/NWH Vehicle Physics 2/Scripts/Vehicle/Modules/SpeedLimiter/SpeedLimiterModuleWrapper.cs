using System;

namespace NWH.VehiclePhysics2.Modules.SpeedLimiter
{
    /// <summary>
    ///     MonoBehaviour wrapper for SpeedLimiter module.
    /// </summary>
    [Serializable]
    public class SpeedLimiterModuleWrapper : ModuleWrapper
    {
        public SpeedLimiterModule module = new SpeedLimiterModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as SpeedLimiterModule;
        }
    }
}