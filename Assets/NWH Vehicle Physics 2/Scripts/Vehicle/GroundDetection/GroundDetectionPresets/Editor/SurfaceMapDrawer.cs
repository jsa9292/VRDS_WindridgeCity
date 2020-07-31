using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.GroundDetection
{
    [CustomPropertyDrawer(typeof(SurfaceMap))]
    public class SurfaceMapDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("name");
            drawer.Field("surfacePreset");
            drawer.ReorderableList("terrainTextureIndices");
            drawer.ReorderableList("tags");

            drawer.EndProperty();
            return true;
        }
    }
}