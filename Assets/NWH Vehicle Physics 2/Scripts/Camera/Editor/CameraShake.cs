using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Cameras
{
    [CustomEditor(typeof(CameraShake))]
    [CanEditMultipleObjects]
    public class CameraShakeEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }
            
            drawer.Field("shakeAmount");
            drawer.Field("shakeDuration");

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}