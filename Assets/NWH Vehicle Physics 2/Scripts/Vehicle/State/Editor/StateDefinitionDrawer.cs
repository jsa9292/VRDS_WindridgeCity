using System.Linq;
using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     Custom property drawer for StateDefinition.
    /// </summary>
    [CustomPropertyDrawer(typeof(StateDefinition))]
    public class StateDefinitionDrawer : NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            drawer.BeginProperty(position, property, label);

            // Draw label
            string fullName = drawer.FindProperty("fullName").stringValue.Replace("NWH.VehiclePhysics.", "");
            string shortName = fullName.Split('.').Last();

            GUIStyle miniStyle = EditorStyles.centeredGreyMiniLabel;
            miniStyle.alignment = TextAnchor.MiddleLeft;

            Rect labelRect = drawer.positionRect;
            labelRect.x += 5f;

            Rect miniRect = drawer.positionRect;
            miniRect.x += 200f;

            EditorGUI.LabelField(labelRect, shortName, EditorStyles.boldLabel);
            EditorGUI.LabelField(miniRect, fullName, miniStyle);
            drawer.Space(NUISettings.FIELD_HEIGHT);

            StateSettings stateSettings =
                SerializedPropertyHelper.GetTargetObjectWithProperty(property) as StateSettings;
            if (stateSettings == null)
            {
                drawer.EndProperty();
                return false;
            }

            ComponentNUIPropertyDrawer.DrawStateSettingsBar
            (
                position,
                stateSettings.LODs.Count,
                property.FindPropertyRelative("isOn"),
                property.FindPropertyRelative("isEnabled"),
                property.FindPropertyRelative("lodIndex")
            );

            drawer.EndProperty();
            return true;
        }
    }
}