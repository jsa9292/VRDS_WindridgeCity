using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Editor for MobileInputProvider.
    /// </summary>
    [CustomEditor(typeof(MobileInputProvider))]
    public class MobileInputProviderEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Info("None of the buttons are mandatory. If you do not wish to use an input leave the field empty.");

            MobileInputProvider mip = target as MobileInputProvider;
            if (mip == null)
            {
                drawer.EndEditor(this);
                return false;
            }

            drawer.BeginSubsection("Input Type");
            drawer.Field("horizontalInputType");
            drawer.Field("verticalInputType");
            drawer.EndSubsection();

            drawer.BeginSubsection("Direction");
            if (mip.horizontalInputType == MobileInputProvider.HorizontalAxisType.SteeringWheel)
            {
                drawer.Field("steeringWheel");
            }

            if (mip.horizontalInputType == MobileInputProvider.HorizontalAxisType.Accelerometer ||
                mip.verticalInputType == MobileInputProvider.VerticalAxisType.Accelerometer)
            {
                drawer.Field("tiltSensitivity");
            }

            if (mip.horizontalInputType == MobileInputProvider.HorizontalAxisType.Button)
            {
                drawer.Field("steerLeftButton");
                drawer.Field("steerRightButton");
            }

            if (mip.verticalInputType == MobileInputProvider.VerticalAxisType.Button)
            {
                drawer.Field("throttleButton");
                drawer.Field("brakeButton");
            }

            drawer.EndSubsection();

            drawer.BeginSubsection("Scene Buttons");
            drawer.Field("changeVehicleButton");
            drawer.Field("changeCameraButton");
            drawer.EndSubsection();

            drawer.BeginSubsection("Vehicle Buttons");
            drawer.Field("changeVehicleButton");
            drawer.Field("engineStartStopButton");
            drawer.Field("handbrakeButton");
            drawer.Field("shiftUpButton");
            drawer.Field("shiftDownButton");
            drawer.Field("extraLightsButton");
            drawer.Field("highBeamLightsButton");
            drawer.Field("lowBeamLightsButton");
            drawer.Field("hazardLightsButton");
            drawer.Field("leftBlinkerButton");
            drawer.Field("rightBlinkerButton");
            drawer.Field("hornButton");
            drawer.Field("trailerAttachDetachButton");
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