using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.GroundDetection
{
    /// <summary>
    ///     Property drawer for Brakes.
    /// </summary>
    [CustomPropertyDrawer(typeof(Brakes))]
    public class BrakesDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Braking");
            drawer.Field("maxTorque", true, "Nm");
            drawer.Field("smoothing");
            drawer.Field("brakeWhileIdle");
            drawer.Field("brakeWhileAsleep");
            drawer.Field("airBrakes");
            drawer.Field("_isBraking", false, null, "Is Braking");
            drawer.EndSubsection();

            drawer.BeginSubsection("Reverse Direction");
            drawer.Field("brakeOnReverseDirection");
            drawer.Field("reverseDirectionVelocityThreshold", true, "m/s");
            drawer.EndSubsection();

            drawer.BeginSubsection("Off-Throttle Braking");
            drawer.Field("brakeOffThrottle");
            drawer.Field("brakeOffThrottleStrength");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}