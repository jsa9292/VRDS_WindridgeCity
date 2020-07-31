using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.CruiseControl
{
    [CustomPropertyDrawer(typeof(CruiseControlModule))]
    public class CruiseControlModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Speed");
            drawer.Field("targetSpeed");
            drawer.Field("setTargetSpeedOnEnable");
            drawer.EndSubsection();

            drawer.BeginSubsection("PID Controller Settings");
            drawer.Field("Kp");
            drawer.Field("Ki");
            drawer.Field("Kd");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}