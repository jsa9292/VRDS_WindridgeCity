using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Aerodynamics
{
    [CustomPropertyDrawer(typeof(AerodynamicsModule))]
    public class AerodynamicsModuleDrawer : ModuleDrawer
    {
        private float infoHeight;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Drag");
            drawer.Field("simulateDrag");
            drawer.Field("frontalCd");
            drawer.Field("sideCd");
            drawer.Field("longitudinalDragForce", false, "N");
            drawer.Field("lateralDragForce", false, "N");
            drawer.Info("To change vehicle dimensions go to 'Other > Settings' tab.");
            drawer.EndSubsection();

            drawer.BeginSubsection("Downforce");
            drawer.Field("simulateDownforce");
            drawer.Field("maxDownforceSpeed", true, "m/s");
            drawer.ReorderableList("downforcePoints");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}