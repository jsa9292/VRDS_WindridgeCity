using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Cameras
{
    [CustomEditor(typeof(CameraMouseDrag))]
    [CanEditMultipleObjects]
    public class CameraMouseDragEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("target");
            
            drawer.BeginSubsection("Distance");
            drawer.Field("distance");
            drawer.Field("minDistance");
            drawer.Field("maxDistance");
            drawer.Field("zoomSensitivity");
            drawer.EndSubsection();

            drawer.BeginSubsection("Position");
            drawer.Field("targetPositionOffset");
            drawer.EndSubsection();

            drawer.BeginSubsection("Rotation");
            drawer.Field("allowRotation");
            drawer.Field("followTargetsRotation");
            drawer.Field("rotationSensitivity");
            drawer.Field("verticalMaxAngle");
            drawer.Field("verticalMinAngle");
            drawer.Field("initXRotation");
            drawer.Field("initYRotation");
            drawer.Field("rotationSmoothing");
            drawer.EndSubsection();
            
            drawer.BeginSubsection("Panning");
            drawer.Field("allowPanning");
            drawer.Field("panningSensitivity");

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}