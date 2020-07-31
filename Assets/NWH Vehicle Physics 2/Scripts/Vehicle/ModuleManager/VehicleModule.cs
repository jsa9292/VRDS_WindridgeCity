using System;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
    /// <summary>
    ///     Base class for all vehicle modules.
    /// </summary>
    [DisallowMultipleComponent]
    [Serializable]
    public abstract class VehicleModule : VehicleComponent
    {
        /// <summary>
        ///     Category of the module.
        /// </summary>
        public enum ModuleCategory
        {
            Other,
            Vehicle,
            DrivingAssists,
            Aero,
            Sound,
            Effects,
            Trailer,
            Animation,
            Control,
            Powertrain
        }

        /// <summary>
        ///     Returns a category the module is in. Used for filtering modules in the inspector to prevent clutter.
        /// </summary>
        /// <returns></returns>
        public abstract ModuleCategory GetModuleCategory();
    }
}