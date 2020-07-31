using System;
using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.ModuleTemplate
{
    /// <summary>
    ///     Empty module example / template.
    /// </summary>
    [Serializable]
    public class ModuleTemplate : VehicleModule
    {
        // EXAMPLE FIELDS

        /// <summary>
        ///     Example float field.
        /// </summary>
        [Range(0, 1)]
        [Tooltip("    Example float field.")]
        public float floatExample;

        /// <summary>
        ///     Example list field.
        /// </summary>
        [Tooltip("    Example list field.")]
        public List<int> listExample = new List<int>();

        public override void Initialize()
        {
            initialized = true;
        }

        public override void FixedUpdate()
        {
            if (!Active)
            {
            }

            // FixedUpdate code here.
        }

        public override void Update()
        {
            if (!Active)
            {
            }

            // Update code here.
        }

        public override ModuleCategory GetModuleCategory()
        {
            return ModuleCategory.Other;
        }
    }
}