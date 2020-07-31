using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Utility
{
    [CustomEditor(typeof(VehicleReflectionProbe))]
    [CanEditMultipleObjects]
    public class VehicleReflectionProbeEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("awakeProbeType");
            drawer.Field("asleepProbeType");
            drawer.Field("bakeOnStart");
            drawer.Field("bakeOnSleep");
            
            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}