using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain.Wheel
{
    [CustomPropertyDrawer(typeof(WheelGroup))]
    public class WheelGroupDrawer : NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("General");
            drawer.Field("name");
            drawer.EndSubsection();

            drawer.BeginSubsection("Steering");
            drawer.Field("steerCoefficient");
            drawer.Field("ackermanPercent");
            drawer.EndSubsection();

            drawer.BeginSubsection("Brakes");
            drawer.Field("brakeCoefficient");
            drawer.Field("handbrakeCoefficient");
            drawer.EndSubsection();

            drawer.BeginSubsection("Geometry");
            drawer.Field("toeAngle", true, "deg");
            drawer.Field("casterAngle", true, "deg");
            drawer.Field("camberAtTop", true, "deg");
            drawer.Field("camberAtBottom", true, "deg");
            drawer.EndSubsection();

            drawer.BeginSubsection("Axle");
            drawer.Field("antiRollBarForce", true, "Nm");
            drawer.Field("isSolid");
            drawer.Info(
                "Fields 'Anti Roll Bar Force' and 'Axle Is Solid' will only work if wheel group has two wheels - a left and a right one.");
            drawer.EndSubsection();


            drawer.EndProperty();
            return true;
        }
    }
}