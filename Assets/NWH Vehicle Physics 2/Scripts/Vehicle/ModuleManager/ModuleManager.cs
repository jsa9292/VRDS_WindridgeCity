using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

#endif

namespace NWH.VehiclePhysics2.Modules
{
    /// <summary>
    ///     Manages vehicle modules.
    /// </summary>
    [Serializable]
    public class ModuleManager : VehicleComponent
    {
        /// <summary>
        ///     Vehicle modules. Only modules in this list will get updated.
        /// </summary>
        [Tooltip("    Vehicle modules. Only modules in this list will get updated.")]
        public List<VehicleModule> modules = new List<VehicleModule>();

        public override void Initialize()
        {
            initialized = true;
        }

        public override void Awake(VehicleController vc)
        {
            base.Awake(vc);

            ReloadModulesList();

            foreach (VehicleModule module in modules)
            {
                module.Awake(vc);
            }
        }

        public override void FixedUpdate()
        {
            foreach (VehicleModule module in modules)
            {
                module.FixedUpdate();
            }
        }

        public override void Update()
        {
            if (!Active)
            {
                return;
            }

            foreach (VehicleModule module in modules)
            {
                module.Update();
            }
        }

        public override void OnDrawGizmosSelected(VehicleController vc)
        {
            foreach (VehicleModule module in vc.moduleManager.modules)
            {
                module.OnDrawGizmosSelected(vc);
            }
        }

        /// <summary>
        ///     Adds a module to the game object. Equivalent to using AddComponent followed by UpdateModulesList().
        ///     Can be called only in play mode, after the vehicle has been initialized.
        /// </summary>
        public TM AddModule<TW, TM>()
            where TW : ModuleWrapper
            where TM : VehicleModule
        {
            if (vc == null)
            {
                return null;
            }

            VehicleModule module = vc.gameObject.AddComponent<TW>().GetModule();
            modules.Add(module);
            return module as TM;
        }

        /// <summary>
        ///     Returns a module from the modules list.
        ///     Can be called only in play mode, after the vehicle has been initialized.
        /// </summary>
        public TM GetModule<TM>() where TM : VehicleModule
        {
            if (vc == null)
            {
                return null;
            }

            return modules.FirstOrDefault(m => m.GetType() == typeof(TM)) as TM;
        }

        public override void CheckState(int lodIndex)
        {
            foreach (VehicleModule module in modules)
            {
                module.CheckState(lodIndex);
            }

            base.CheckState(lodIndex);
        }

        /// <summary>
        ///     Removes the module from the object and from the modules list.
        ///     Can be called only in play mode, after the vehicle has been initialized.
        /// </summary>
        public void RemoveModule<TW>()
            where TW : ModuleWrapper
        {
            if (vc == null)
            {
                return;
            }

            ModuleWrapper wrapper = vc.gameObject.GetComponent<TW>();
            modules.Remove(wrapper.GetModule());
            if (Application.isPlaying)
            {
                Object.Destroy(wrapper);
            }
            else
            {
                Object.DestroyImmediate(wrapper);
            }
        }

        public void ReloadModulesList()
        {
            modules.Clear();
            List<ModuleWrapper> moduleWrappers = vc.GetComponents<ModuleWrapper>().ToList();
            foreach (ModuleWrapper wrapper in moduleWrappers)
            {
                modules.Add(wrapper.GetModule());

                if (Application.isPlaying)
                {
                    wrapper.GetModule().Awake(vc);
                }
            }
        }
    }
}