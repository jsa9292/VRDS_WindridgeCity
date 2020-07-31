using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Cameras
{
    [CustomEditor(typeof(CameraFollow))]
    [CanEditMultipleObjects]
    public class CameraFollowEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("target");

            drawer.BeginSubsection("Positioning");
            drawer.Field("distance");
            drawer.Field("height");
            drawer.Field("smoothing");
            drawer.Field("targetForwardOffset");
            drawer.Field("targetUpOffset");
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