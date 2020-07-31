using System;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
    /// <summary>
    ///     MonoBehaviour wrapper for TrailerHitch module.
    /// </summary>
    [Serializable]
    public class TrailerHitchModuleWrapper : ModuleWrapper
    {
        public TrailerHitchModule module = new TrailerHitchModule();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as TrailerHitchModule;
        }
    }
}