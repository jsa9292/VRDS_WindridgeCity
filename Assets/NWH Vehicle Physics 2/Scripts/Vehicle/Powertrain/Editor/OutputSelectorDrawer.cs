using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(OutputSelector))]
    public class OutputSelectorDrawer : PropertyDrawer
    {
        public int selectedIndex;
        public SerializedProperty socketNameProperty;
        public VehicleController vc;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            List<string> options = new List<string> {"[none]"};

            // Try to get vehicle controller from parent property
            vc = property.serializedObject.targetObject as VehicleController;

            if (vc == null)
            {
                selectedIndex = EditorGUI.Popup(rect, "", 0, options.ToArray());
                return;
            }

            // Get available components (solver is not inialized out of play mode so its component list is empty)
            List<PowertrainComponent> powertrainComponents = vc.powertrain.GetPowertrainComponents();

            // Find the name and index of currently selected output
            socketNameProperty = property.FindPropertyRelative("name");
            string name = socketNameProperty.stringValue;
            int index = powertrainComponents.FindIndex(c => c.name == name) + 1;

            // Add the names of other powertrain components
            options.AddRange(vc.powertrain.GetPowertrainComponentNamesWithType(powertrainComponents));

            // Display dropdown menu
            selectedIndex = EditorGUI.Popup(rect, "", index, options.ToArray());

            // Display currently selected output
            if (selectedIndex >= 1)
            {
                socketNameProperty.stringValue = powertrainComponents[selectedIndex - 1].name;
            }
            else
            {
                socketNameProperty.stringValue = null;
            }
        }
    }
}