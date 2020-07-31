using System;

namespace NWH.VehiclePhysics2.Modules.ESC
{
    /// <summary>
    ///     MonoBehaviour wrapper for Electronic Stability Control (ESC) module.
    /// </summary>
    [Serializable]
    public class ESCModuleWrapper : ModuleWrapper
    {
        public ESCModule module = new ESCModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as ESCModule;
        }
    }
}