using System;

namespace NWH.VehiclePhysics2.Modules.FlipOver
{
    /// <summary>
    ///     MonoBehaviour wrapper for FlipOver module.
    /// </summary>
    [Serializable]
    public class FlipOverModuleWrapper : ModuleWrapper
    {
        public FlipOverModule module = new FlipOverModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as FlipOverModule;
        }
    }
}