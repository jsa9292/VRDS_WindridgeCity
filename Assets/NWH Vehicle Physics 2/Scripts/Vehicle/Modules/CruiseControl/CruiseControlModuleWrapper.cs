using System;

namespace NWH.VehiclePhysics2.Modules.CruiseControl
{
    /// <summary>
    ///     MonoBehaviour wrapper for CruiseControl module.
    /// </summary>
    [Serializable]
    public class CruiseControlModuleWrapper : ModuleWrapper
    {
        public CruiseControlModule module = new CruiseControlModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as CruiseControlModule;
        }
    }
}