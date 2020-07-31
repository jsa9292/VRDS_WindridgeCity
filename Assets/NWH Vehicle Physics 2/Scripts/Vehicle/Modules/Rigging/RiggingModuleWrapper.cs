using System;

namespace NWH.VehiclePhysics2.Modules.Rigging
{
    /// <summary>
    ///     MonoBehaviour wrapper for Rigging module.
    /// </summary>
    [Serializable]
    public class RiggingModuleWrapper : ModuleWrapper
    {
        public RiggingModule module = new RiggingModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as RiggingModule;
        }
    }
}