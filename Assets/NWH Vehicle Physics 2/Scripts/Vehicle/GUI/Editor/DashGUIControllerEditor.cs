using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.VehicleGUI
{
    [CustomEditor(typeof(DashGUIController))]
    [CanEditMultipleObjects]
    public class DashGUIControllerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            if (drawer.Field("dataSource").enumValueIndex == 0)
            {
                drawer.Field("vehicleController");
            }
            else
            {
                drawer.Info(
                    "VehicleChanger is being used to get the active vehicle. Make sure you have one VehicleChanger present in your scene.");
            }

            drawer.BeginSubsection("Gauges");
            if (drawer.Field("useAnalogRpmGauge").boolValue)
            {
                drawer.Field("analogRpmGauge");
            }

            if (drawer.Field("useDigitalRpmGauge").boolValue)
            {
                drawer.Field("digitalRpmGauge");
            }

            if (drawer.Field("useAnalogSpeedGauge").boolValue)
            {
                drawer.Field("analogSpeedGauge");
            }

            if (drawer.Field("useDigitalSpeedGauge").boolValue)
            {
                drawer.Field("digitalSpeedGauge");
            }

            if (drawer.Field("useDigitalGearGauge").boolValue)
            {
                drawer.Field("digitalGearGauge");
            }

            drawer.EndSubsection();

            drawer.BeginSubsection("Dash Lights");
            if (drawer.Field("useLeftBlinkerDashLight").boolValue)
            {
                drawer.Field("leftBlinkerDashLight");
            }

            if (drawer.Field("useRightBlinkerDashLight").boolValue)
            {
                drawer.Field("rightBlinkerDashLight");
            }

            if (drawer.Field("useLowBeamDashLight").boolValue)
            {
                drawer.Field("lowBeamDashLight");
            }

            if (drawer.Field("useHighBeamDashLight").boolValue)
            {
                drawer.Field("highBeamDashLight");
            }

            if (drawer.Field("useTcsDashLight").boolValue)
            {
                drawer.Field("tcsDashLight");
            }

            if (drawer.Field("useAbsDashLight").boolValue)
            {
                drawer.Field("absDashLight");
            }

            if (drawer.Field("useCheckEngineDashLight").boolValue)
            {
                drawer.Field("checkEngineDashLight");
            }

            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}