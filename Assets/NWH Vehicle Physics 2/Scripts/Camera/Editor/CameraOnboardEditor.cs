using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Cameras
{
    [CustomEditor(typeof(CameraOnboard))]
    [CanEditMultipleObjects]
    public class CameraOnboardEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("target");

            drawer.BeginSubsection("Positioning");
            drawer.Field("maxMovementOffset");
            drawer.Field("movementIntensity");
            drawer.Field("movementSmoothing");
            drawer.Field("axisIntensity");
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