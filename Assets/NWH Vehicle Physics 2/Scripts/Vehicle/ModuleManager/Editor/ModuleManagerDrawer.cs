using System.Linq;
using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
    [CustomPropertyDrawer(typeof(ModuleManager))]
    public class ModuleManagerDrawer : ComponentNUIPropertyDrawer
    {
        private float infoHeight;
        private int selectedModule;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            VehicleController vehicleController =
                SerializedPropertyHelper.GetTargetObjectWithProperty(property) as VehicleController;
            if (vehicleController == null)
            {
                drawer.EndProperty();
                return false;
            }

            ModuleManager moduleManager = SerializedPropertyHelper.GetTargetObjectOfProperty(property) as ModuleManager;
            if (moduleManager == null)
            {
                drawer.EndProperty();
                return false;
            }

            moduleManager.VehicleController = vehicleController;

            if (!Application.isPlaying && (int) EditorApplication.timeSinceStartup % 2 == 0)
            {
                moduleManager.ReloadModulesList();
            }

            drawer.Space();

            ModuleWrapper[] wrappers = vehicleController.gameObject.GetComponents<ModuleWrapper>();
            if (wrappers.Length == 0)
            {
                drawer.Info("Use 'Add Component' button to add a module. Modules will appear here as they are added.");
                drawer.EndProperty();
                return true;
            }

            drawer.Label("Module Categories:");
            VehicleModule.ModuleCategory[] moduleCategories =
                moduleManager.modules.Select(m => m.GetModuleCategory()).Distinct().OrderBy(x => x).ToArray();
            int categoryIndex =
                drawer.HorizontalToolbar("moduleCategories", moduleCategories.Select(m => m.ToString()).ToArray());
            if (categoryIndex < 0)
            {
                categoryIndex = 0;
            }

            if (categoryIndex >= moduleCategories.Length)
            {
                drawer.EndProperty();
                return true;
            }

            drawer.Space(3);
            VehicleModule.ModuleCategory activeCategory = moduleCategories[categoryIndex];

            foreach (ModuleWrapper wrapper in wrappers)
            {
                if (wrapper == null || wrapper.GetModule() == null)
                {
                    continue;
                }

                if (wrapper.GetModule().GetModuleCategory() != activeCategory)
                {
                    continue;
                }

                drawer.EmbeddedObjectEditor(wrapper, drawer.positionRect);
            }

            drawer.EndProperty();
            return true;
        }
    }
}