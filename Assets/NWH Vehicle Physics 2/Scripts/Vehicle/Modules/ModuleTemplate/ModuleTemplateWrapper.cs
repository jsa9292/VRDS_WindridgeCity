using System;

namespace NWH.VehiclePhysics2.Modules.ModuleTemplate
{
    /// <summary>
    ///     MonoBehaviour wrapper for example module.
    /// </summary>
    [Serializable]
    public class ModuleTemplateWrapper : ModuleWrapper
    {
        public ModuleTemplate module = new ModuleTemplate();

        public override VehicleModule GetModule()
        {
            return module;
        }

        public override void SetModule(VehicleModule module)
        {
            this.module = module as ModuleTemplate;
        }
    }
}