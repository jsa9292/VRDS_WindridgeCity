using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Rigging
{
    [CustomPropertyDrawer(typeof(RiggingModule))]
    public class RiggingModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.ReorderableList("bones");

            drawer.EndProperty();
            return true;
        }
    }
}