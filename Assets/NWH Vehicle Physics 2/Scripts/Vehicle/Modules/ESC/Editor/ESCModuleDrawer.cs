using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.ESC
{
    [CustomPropertyDrawer(typeof(ESCModule))]
    public class ESCModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Info("ECS only works with 4-wheel vehicles.");
            drawer.Field("intensity");
            drawer.Field("lowerSpeedThreshold");

            drawer.EndProperty();
            return true;
        }
    }
}