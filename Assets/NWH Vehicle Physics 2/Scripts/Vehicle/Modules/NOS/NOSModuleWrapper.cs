using System;

namespace NWH.VehiclePhysics2.Modules.NOS
{
    /// <summary>
    ///     MonoBehaviour wrapper for NOS module.
    /// </summary>
    [Serializable]
    public class NOSModuleWrapper : ModuleWrapper
    {
        public NOSModule module = new NOSModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as NOSModule;
        }
    }
}