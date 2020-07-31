using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.SpeedLimiter
{
    [CustomPropertyDrawer(typeof(SpeedLimiterModule))]
    public class SpeedLimiterModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("speedLimit");
            drawer.Field("speedUnits");
            drawer.Field("active", false);

            drawer.EndProperty();
            return true;
        }
    }
}