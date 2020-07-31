using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.ABS
{
    [CustomPropertyDrawer(typeof(ABSModule))]
    public class ABSModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("slipThreshold");
            drawer.Field("lowerSpeedThreshold", true, "m/s");
            drawer.Field("active", false);
            drawer.Field("ABSActive");

            drawer.EndProperty();
            return true;
        }
    }
}