using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.GroundDetection
{
    [CustomEditor(typeof(GroundDetectionPreset))]
    [CanEditMultipleObjects]
    public class GroundDetectionPresetEditor : NUIEditor
    {
        private GroundDetectionPreset preset;

        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            preset = (GroundDetectionPreset) target;

            drawer.BeginSubsection("Particles");
            drawer.Field("particlePrefab");
            drawer.Field("chunkPrefab");
            drawer.EndSubsection();

            drawer.BeginSubsection("Surface Maps");
            drawer.Field("fallbackSurfacePreset");
            drawer.ReorderableList("surfaceMaps");
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }
}