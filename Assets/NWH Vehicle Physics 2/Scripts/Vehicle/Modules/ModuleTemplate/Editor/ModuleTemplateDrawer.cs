using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.ModuleTemplate
{
    [CustomPropertyDrawer(typeof(ModuleTemplate))]
    public class ModuleTemplateDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("floatExample");

            drawer.BeginSubsection("Subsection Example");
            drawer.ReorderableList("listExample");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}