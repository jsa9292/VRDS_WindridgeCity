using System;

namespace NWH.VehiclePhysics2.Modules.Fuel
{
    /// <summary>
    ///     MonoBehaviour wrapper for Fuel module.
    /// </summary>
    [Serializable]
    public class FuelModuleWrapper : ModuleWrapper
    {
        public FuelModule module = new FuelModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as FuelModule;
        }
    }
}