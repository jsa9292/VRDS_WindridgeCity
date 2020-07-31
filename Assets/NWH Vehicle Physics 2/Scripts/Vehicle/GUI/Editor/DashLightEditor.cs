using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.VehicleGUI
{
    [CustomEditor(typeof(DashLight))]
    [CanEditMultipleObjects]
    public class DashLightEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("onColor");
            drawer.Field("offColor");
            drawer.Field("fadeTime");
            drawer.Field("holdTime");

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}