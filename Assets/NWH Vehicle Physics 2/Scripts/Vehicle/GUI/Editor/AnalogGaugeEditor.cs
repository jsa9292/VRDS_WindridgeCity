using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.VehicleGUI
{
    [CustomEditor(typeof(AnalogGauge))]
    [CanEditMultipleObjects]
    public class AnalogGaugeEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("maxValue");
            drawer.Field("startAngle");
            drawer.Field("endAngle");
            drawer.Field("needleSmoothing");
            drawer.Field("lockAtStart");
            drawer.Field("lockAtEnd");
            drawer.Info("LockAtStart and LockAtEnd only work in play mode.");

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}