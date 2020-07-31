using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(DifferentialComponent))]
    public class DifferentialComponentDrawer : PowertrainComponentDrawer
    {
        public override void DrawPowertrainOutputSection(ref Rect rect, SerializedProperty property)
        {
            drawer.Field("outputASelector");
            drawer.Field("outputBSelector");
        }

        public override int GetOutputCount()
        {
            return 2;
        }

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            DrawCommonProperties();

            drawer.BeginSubsection("Differential Settings");
            drawer.Field("differentialType");

            int typeEnumValue = property.FindPropertyRelative("differentialType").enumValueIndex;
            if (typeEnumValue != (int) DifferentialComponent.Type.External)
            {
                drawer.Field("biasAB");

                if (typeEnumValue != (int) DifferentialComponent.Type.Open)
                {
                    drawer.Field("stiffness");

                    if (typeEnumValue != (int) DifferentialComponent.Type.Locked)
                    {
                        drawer.Field("slipTorque");
                        drawer.Field("preload");
                        drawer.Field("powerRamp");
                        drawer.Field("coastRamp");
                    }
                }
            }
            else
            {
                drawer.Info(
                    "Using differential from external script. Check the script for settings. If no torque split delegate is assigned, " +
                    "differentiall will fall back to Open type.");
            }

            drawer.EndSubsection();
            drawer.EndProperty();
            return true;
        }
    }
}