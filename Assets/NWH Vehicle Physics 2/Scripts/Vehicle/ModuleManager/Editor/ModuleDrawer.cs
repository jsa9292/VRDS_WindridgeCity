using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
    [CustomPropertyDrawer(typeof(VehicleModule), false)]
    public class ModuleDrawer : ComponentNUIPropertyDrawer
    {
        public VehicleModule VehicleModule;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            drawer.BeginProperty(position, property, label);

            VehicleModule =
                SerializedPropertyHelper.GetTargetObjectOfProperty(drawer.serializedProperty) as VehicleModule;
            if (VehicleModule == null)
            {
                Debug.LogError(
                    "VehicleModule is not a target object of ModuleDrawer. Make sure all modules inherit from VehicleModule.");
                drawer.EndProperty();
                return false;
            }


            bool expanded = drawer.Header(VehicleModule.GetType().Name);

            if (Application.isPlaying && VehicleModule.VehicleController != null)
            {
                if (VehicleModule.VehicleController.stateSettings != null)
                {
                    DrawStateSettingsBar(
                        position,
                        VehicleModule.VehicleController.stateSettings.LODs.Count,
                        drawer.FindProperty("state.isOn"),
                        drawer.FindProperty("state.isEnabled"),
                        drawer.FindProperty("state.lodIndex"));
                }
            }

            return expanded;
        }
    }
}