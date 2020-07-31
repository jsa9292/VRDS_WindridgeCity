using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomEditor(typeof(TransmissionGearingProfile))]
    [CanEditMultipleObjects]
    public class TransmissionGearingProfileEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.BeginSubsection("Gear Ratios");
            drawer.ReorderableList("forwardGears");
            drawer.ReorderableList("reverseGears");
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }
}