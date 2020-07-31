using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Input
{
    [CustomEditor(typeof(DesktopInputProvider))]
    public class DesktopInputProviderEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }
            
            drawer.Field("inputType");
            drawer.Field("verticalInputMapping");

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}