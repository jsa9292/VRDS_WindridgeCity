using System.Collections.Generic;
using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain.Wheel
{
    [CustomPropertyDrawer(typeof(WheelGroupSelector))]
    public class WheelGroupSelectorDrawer : NUIPropertyDrawer
    {
        public SerializedProperty groupIndexProperty;
        public int selectedIndex;
        public VehicleController vc;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return NUISettings.FIELD_HEIGHT;
        }

        public override bool OnNUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            List<string> options = new List<string> {"[no axle]"};

            // Try to get vehicle controller from parent property
            vc = property.serializedObject.targetObject as VehicleController;

            if (vc == null)
            {
                selectedIndex = EditorGUI.Popup(rect, "Belongs to", 0, options.ToArray());
                return false;
            }

            // Find the name and index of currently selected output
            groupIndexProperty = property.FindPropertyRelative("index");

            if (groupIndexProperty == null)
            {
                Debug.LogWarning("Property axleIndex not found.");
                return false;
            }

            int axleIndex = groupIndexProperty.intValue;
            int selectorIndex = axleIndex + 1;
            string name = axleIndex >= vc.powertrain.wheelGroups.Count || axleIndex < 0
                ? ""
                : vc.powertrain.wheelGroups[axleIndex].name;

            // Add the names of other powertrain components
            for (int i = 0; i < vc.powertrain.wheelGroups.Count; i++)
            {
                options.Add($"{vc.powertrain.wheelGroups[i].name} [Group {i.ToString()}]");
            }

            // Display dropdown menu
            selectedIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, NUISettings.FIELD_HEIGHT),
                "Belongs to", selectorIndex, options.ToArray());

            // Display currently selected output
            groupIndexProperty.intValue = selectedIndex - 1;


            if (selectedIndex > 0 && selectedIndex <= vc.powertrain.wheelGroups.Count)
            {
                WheelComponent wheelComponent =
                    SerializedPropertyHelper.GetTargetObjectWithProperty(property) as WheelComponent;
                if (wheelComponent != null)
                {
                    wheelComponent.wheelGroup = vc.powertrain.wheelGroups[selectedIndex - 1];
                }
                else
                {
                    Debug.LogWarning("WheelComponent property not found. Wheel group will not be set.");
                }
            }

            return true;
        }
    }
}