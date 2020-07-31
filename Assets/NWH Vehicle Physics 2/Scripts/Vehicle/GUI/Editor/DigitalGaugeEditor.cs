using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.VehicleGUI
{
    [CustomEditor(typeof(DigitalGauge))]
    [CanEditMultipleObjects]
    public class DigitalGaugeEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            if (drawer.Field("gaugeType").enumValueIndex == 0)
            {
                drawer.Field("numericalValue");
                drawer.Field("format");
                drawer.Field("numericalSmoothing");
                if (drawer.Field("showProgressBar").boolValue)
                {
                    drawer.Field("maxValue");
                }
            }
            else
            {
                drawer.Field("stringValue");
            }

            drawer.Field("unit");

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}