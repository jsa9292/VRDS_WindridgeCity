using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(ClutchComponent))]
    public class ClutchComponentDrawer : PowertrainComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            DrawCommonProperties();

            drawer.BeginSubsection("General");
            bool isAutomatic = drawer.FindProperty("isAutomatic").boolValue;
            drawer.Field("clutchEngagement", false);
            if (isAutomatic)
            {
                drawer.Info(
                    "Clutch engagement is being set automatically according to PID controller settings. IsAutomatic is true.");
            }
            else
            {
                drawer.Info(
                    "Clutch engagement is being set through user input. Check input settings for 'Clutch' axis.");
            }

            drawer.Field("slipTorque", true, "Nm");
            drawer.EndSubsection();

            drawer.BeginSubsection("Automatic Clutch");
            if (drawer.Field("isAutomatic").boolValue)
            {
                drawer.Field("baseEngagementRPM");
                drawer.Field("variableEngagementRPMRange");
                drawer.Field("finalEngagementRPM", false);

                drawer.BeginSubsection("PID Controller");
                drawer.Field("PID_Kp");
                drawer.Field("PID_Ki");
                drawer.Field("PID_Kd");
                drawer.Field("PID_Coefficient");
                drawer.EndSubsection();
            }

            drawer.EndSubsection();

            drawer.BeginSubsection("Torque Converter");
            if (drawer.Field("hasTorqueConverter").boolValue)
            {
                drawer.Field("torqueConverterSlipTorque");
            }

            drawer.EndSubsection();
            drawer.EndProperty();
            return true;
        }
    }
}