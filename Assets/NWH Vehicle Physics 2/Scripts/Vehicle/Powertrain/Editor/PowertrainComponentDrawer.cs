using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
    public class PowertrainComponentDrawer : NUIPropertyDrawer
    {
        public void DrawCommonProperties()
        {
            drawer.BeginSubsection("Common Properties");
            drawer.Field("name");
            drawer.Field("inertia");
            drawer.Label("Output To:");
            DrawPowertrainOutputSection(ref drawer.positionRect, drawer.serializedProperty);
            drawer.EndSubsection();
        }

        public virtual void DrawPowertrainOutputSection(ref Rect rect, SerializedProperty property)
        {
            drawer.Field("outputASelector");
        }

        public virtual int GetOutputCount()
        {
            return 1;
        }
    }
}