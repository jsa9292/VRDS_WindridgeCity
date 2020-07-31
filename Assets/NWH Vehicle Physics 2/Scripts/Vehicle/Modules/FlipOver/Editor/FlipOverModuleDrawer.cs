using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.FlipOver
{
    [CustomPropertyDrawer(typeof(FlipOverModule))]
    public class FlipOverModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("manual");
            drawer.Field("timeout");
            drawer.Field("allowedAngle");
            drawer.Field("maxDetectionSpeed");
            drawer.Field("rotationSpeed");
            drawer.Field("flippedOver", false);

            drawer.EndProperty();
            return true;
        }
    }
}