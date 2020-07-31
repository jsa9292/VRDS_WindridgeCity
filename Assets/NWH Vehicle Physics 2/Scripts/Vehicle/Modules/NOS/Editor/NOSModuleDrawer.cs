using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.NOS
{
    [CustomPropertyDrawer(typeof(NOSModule))]
    public class NOSModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("active", false);
            drawer.Field("capacity");
            drawer.Field("charge");
            drawer.Field("flow");
            drawer.Field("powerCoefficient");
            drawer.Field("exhaustEmissionCoefficient");
            drawer.Field("engineVolumeCoefficient");
            drawer.Field("disableOffThrottle");
            drawer.Field("disableInReverse");
            drawer.Property("soundComponent");

            drawer.EndProperty();
            return true;
        }
    }
}