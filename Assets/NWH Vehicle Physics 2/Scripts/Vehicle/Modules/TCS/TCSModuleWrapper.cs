using System;

namespace NWH.VehiclePhysics2.Modules.TCS
{
    /// <summary>
    ///     MonoBehaviour wrapper for TCS module.
    /// </summary>
    [Serializable]
    public class TCSModuleWrapper : ModuleWrapper
    {
        public TCSModule module = new TCSModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as TCSModule;
        }
    }
}