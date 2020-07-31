using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Utility
{
    [CustomEditor(typeof(FollowVehicleState))]
    [CanEditMultipleObjects]
    public class FollowVehicleStateEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }
            
            drawer.Info("Enables/disables the GameObject based on Vehicle state (awake/asleep).");

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}