using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     Used for drawing VehicleComponent properties.
    ///     Adds state header functionality to the NUIPropertyDrawer.
    /// </summary>
    public class ComponentNUIPropertyDrawer : NUIPropertyDrawer
    {
        public static void DrawStateSettingsBar(Rect position, int lodCount, SerializedProperty isOnProperty,
            SerializedProperty enabledProperty, SerializedProperty lodIndexProperty, float topOffset = 4f)
        {
            Color initialColor = GUI.backgroundColor;

            // Button style
            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fixedHeight = 15;
            buttonStyle.fontSize = 8;
            buttonStyle.padding = new RectOffset(0, 0, buttonStyle.padding.top, buttonStyle.padding.bottom);
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            // DRAW isOn BUTTON
            bool guiWasEnabled = GUI.enabled;

            if (isOnProperty != null)
            {
                bool isOn = isOnProperty.boolValue;
                GUI.backgroundColor = isOn ? NUISettings.enabledColor : NUISettings.disabledColor;
                string text = isOn ? "ON" : "OFF";
                Rect buttonRect = new Rect(position.x + position.width - 40f, position.y + topOffset, 35f, 17f);
                if (GUI.Button(buttonRect, text, buttonStyle))
                {
                    isOn = !isOn;
                    isOnProperty.boolValue = isOn;
                }

                GUI.enabled = guiWasEnabled;
                GUI.backgroundColor = initialColor;

                if (!isOn)
                {
                    return;
                }
            }

            // DRAW LOD BUTTONS
            int lodIndexValue = -1;
            if (lodIndexProperty != null)
            {
                lodIndexValue = lodIndexProperty.intValue;

                // Draw LOD menu
                if (lodCount > 0)
                {
                    if (lodIndexProperty != null)
                    {
                        int lodIndex = lodIndexValue;
                        bool lodActive = lodIndex >= 0;
                        float rightOffset = -94;
                        float lodButtonWidth = 18f;
                        float lodLabelWidth = 35f;
                        float lodWidth = lodCount * lodButtonWidth;

                        /// Draw label
                        Rect lodLabelRect = new Rect(
                            position.x + position.width - lodWidth - lodLabelWidth + rightOffset,
                            position.y + topOffset, lodLabelWidth, 15f);
                        bool wasEnabled = GUI.enabled;
                        GUI.enabled = false;

                        GUIStyle lodLabelStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                        lodLabelStyle.fixedHeight = 15;
                        lodLabelStyle.fontSize = 10;

                        GUI.Button(lodLabelRect, "LOD", lodLabelStyle);

                        GUI.enabled = wasEnabled;

                        // Draw lod buttons
                        initialColor = GUI.backgroundColor;
                        if (lodIndex >= 0)
                        {
                            GUI.backgroundColor = NUISettings.enabledColor;
                        }

                        GUIStyle lodButtonStyle;
                        GUIStyle middleLODButtonStyle = new GUIStyle(EditorStyles.miniButtonMid);
                        GUIStyle lastLODButtonStyle = new GUIStyle(EditorStyles.miniButtonRight);

                        middleLODButtonStyle.fixedHeight = lastLODButtonStyle.fixedHeight = 15;
                        middleLODButtonStyle.fontSize = lastLODButtonStyle.fontSize = 8;
                        middleLODButtonStyle.alignment = lastLODButtonStyle.alignment = TextAnchor.MiddleCenter;

                        for (int i = 0; i < lodCount; i++)
                        {
                            Rect lodButtonRect = new Rect(
                                position.x + position.width - lodWidth + i * lodButtonWidth +
                                rightOffset,
                                position.y + topOffset, lodButtonWidth, 15f);

                            string buttonText = i.ToString();
                            if (i == lodCount - 1)
                            {
                                buttonText = "S";
                            }

                            lodButtonStyle = i == lodCount - 1 ? lastLODButtonStyle : middleLODButtonStyle;

                            if (GUI.Button(lodButtonRect, buttonText, lodButtonStyle))
                            {
                                if (i == lodIndex)
                                {
                                    lodIndexProperty.intValue = -1;
                                }
                                else
                                {
                                    lodIndexProperty.intValue = i;
                                }
                            }

                            if (i == lodIndex)
                            {
                                GUI.backgroundColor = NUISettings.disabledColor;
                            }
                        }

                        GUI.backgroundColor = initialColor;
                    }
                }
            }


            // Draw Enabled button
            if (enabledProperty != null)
            {
                guiWasEnabled = GUI.enabled;

                if (lodIndexValue < 0)
                {
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                }

                bool enabled = enabledProperty.boolValue;
                GUI.backgroundColor = enabled ? NUISettings.enabledColor : NUISettings.disabledColor;
                string text = enabled ? "ENABLED" : "DISABLED";
                Rect buttonRect = new Rect(position.x + position.width - 89f, position.y + topOffset, 45f, 17f);
                if (GUI.Button(buttonRect, text, buttonStyle))
                {
                    enabled = !enabled;
                    enabledProperty.boolValue = enabled;
                }

                GUI.enabled = guiWasEnabled;
                GUI.backgroundColor = initialColor;
            }
        }

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isExpanded = base.OnNUI(position, property, label);

            VehicleController vehicleController = property.serializedObject.targetObject as VehicleController;

            if (Application.isPlaying)
            {
                if (vehicleController != null && vehicleController.stateSettings != null)
                {
                    DrawStateSettingsBar(
                        position,
                        vehicleController.stateSettings.LODs.Count,
                        property.FindPropertyRelative("state.isOn"),
                        property.FindPropertyRelative("state.isEnabled"),
                        property.FindPropertyRelative("state.lodIndex"));
                }
            }

            return isExpanded;
        }
    }
}