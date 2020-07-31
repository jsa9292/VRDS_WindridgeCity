using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
    /// <summary>
    ///     Wrapper around modules.
    ///     Unity does not support polymorphic serializations (not counting the SerializeReference which in 2019.3 is not
    ///     production ready) so
    ///     the workaround is to wrap each module type in a MonoBehaviour wrapper.
    /// </summary>
    [Serializable]
    public abstract class ModuleWrapper : MonoBehaviour
    {
        /// <summary>
        ///     Returns the category of the module.
        /// </summary>
        public virtual VehicleModule.ModuleCategory GetCategory()
        {
            return GetModule().GetModuleCategory();
        }

        /// <summary>
        ///     Returns wrapper's module.
        /// </summary>
        /// <returns></returns>
        public abstract VehicleModule GetModule();

        /// <summary>
        ///     Sets wrapper's module.
        /// </summary>
        /// <param name="vehicleModule"></param>
        public abstract void SetModule(VehicleModule vehicleModule);
    }
}