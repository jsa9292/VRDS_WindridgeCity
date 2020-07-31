using System;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
    /// <summary>
    ///     MonoBehaviour wrapper for Trailer module.
    /// </summary>
    [Serializable]
    public class TrailerModuleWrapper : ModuleWrapper
    {
        public TrailerModule module = new TrailerModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as TrailerModule;
        }
    }
}