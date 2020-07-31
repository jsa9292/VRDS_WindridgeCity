using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Cameras
{
    [CustomEditor(typeof(CameraChanger))]
    [CanEditMultipleObjects]
    public class CameraChangerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("currentCameraIndex");
            if (drawer.Field("autoFindCameras").boolValue)
            {
                drawer.Info(
                    "When using autoFindCameras make sure that all the cameras are direct children of the object this script is attached to.");
            }
            else
            {
                drawer.ReorderableList("vehicleCameras");
            }

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}