using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.GroundDetection
{
    /// <summary>
    ///     Editor for LOD.
    /// </summary>
    [CustomEditor(typeof(LOD))]
    [CanEditMultipleObjects]
    public class LODEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.BeginSubsection("Settings");
            drawer.Field("distance");
            drawer.Field("singleRayGroundDetection");
            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }
    }
}