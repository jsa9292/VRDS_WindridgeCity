using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(ExhaustFlash))]
    public class ExhaustFlashDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("flashOnRevLimiter");
            drawer.Field("flashOnShift");
            drawer.ReorderableList("meshRenderers");
            drawer.ReorderableList("flashTextures");
            drawer.ReorderableList("flashLights");

            drawer.EndProperty();
            return true;
        }
    }
}