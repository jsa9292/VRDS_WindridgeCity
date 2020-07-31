using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(WheelComponent))]
    public class WheelComponentDrawer : PowertrainComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            DrawCommonProperties();

            drawer.BeginSubsection("Wheel Settings");
            drawer.Field("wheelGroupSelector");
            drawer.Field("wheelController");

            //SerializedProperty wheelControllerProperty = drawer.FindProperty("wheelController");
            //drawer.EmbeddedObjectEditor(SerializedPropertyHelper.GetTargetObjectOfProperty(wheelControllerProperty) as Object,
            //    drawer.positionRect);
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}