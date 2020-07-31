using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(VehicleLight))]
    public class VehicleLightDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection(property.displayName);
            drawer.ReorderableList("lightSources");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}