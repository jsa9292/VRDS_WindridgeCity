using System;

namespace NWH.VehiclePhysics2.Modules.Aerodynamics
{
    /// <summary>
    ///     MonoBehaviour wrapper for Aerodynamics module.
    /// </summary>
    [Serializable]
    public class AerodynamicsModuleWrapper : ModuleWrapper
    {
        public AerodynamicsModule module = new AerodynamicsModule();

        public override VehicleModule.ModuleCategory GetCategory()
        {
            return module.GetModuleCategory();
        }

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as AerodynamicsModule;
        }
    }
}