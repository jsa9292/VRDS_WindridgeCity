using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     Property drawer for Steering.
    /// </summary>
    [CustomPropertyDrawer(typeof(Steering))]
    public class SteeringDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Steer Angle");
            drawer.Field("maximumSteerAngle");
            if(!drawer.Field("useDirectInput").boolValue)
            {
                drawer.Field("speedSensitiveSteeringCurve");
                drawer.Field("linearity");
                drawer.Field("smoothing");
                drawer.Field("degreesPerSecondLimit");
                drawer.Field("returnToCenter");
            }
            drawer.EndSubsection();

            drawer.BeginSubsection("Animation");
            drawer.Field("steeringWheel");
            drawer.Field("steeringWheelTurnRatio");
            drawer.Info("Use negative turn ratio to turn the wheel in opposite direction if needed.");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}