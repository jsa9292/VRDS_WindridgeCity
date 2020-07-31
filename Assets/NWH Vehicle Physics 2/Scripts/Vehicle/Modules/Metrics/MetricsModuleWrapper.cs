using System;

namespace NWH.VehiclePhysics2.Modules.Metrics
{
    /// <summary>
    ///     MonoBehaviour wrapper for Metrics module.
    /// </summary>
    [Serializable]
    public class MetricsModuleWrapper : ModuleWrapper
    {
        public MetricsModule module = new MetricsModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as MetricsModule;
        }
    }
}