using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(EngineComponent))]
    public class EngineComponentDrawer : PowertrainComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            DrawCommonProperties();

            int typeEnumValue = property.FindPropertyRelative("engineType").enumValueIndex;
            bool isElectric = typeEnumValue == (int) EngineComponent.EngineType.Electric;

            drawer.BeginSubsection("General");
            drawer.Field("engineType", !Application.isPlaying);
            //drawer.Field("minRPM", true, "rpm");
            if (!isElectric)
            {
                if (drawer.Field("stallingEnabled").boolValue)
                {
                    drawer.Field("stallRPM", true, "rpm");
                }
            }

            //drawer.Field("maxRPM", true, "rpm");
            drawer.Field("ignition");
            if (!isElectric)
            {
                drawer.Field("autoStartOnThrottle");
            }

            //drawer.Field("engineLayout");
            drawer.EndSubsection();


            drawer.BeginSubsection("Power & Torque");
            drawer.Field("maxPower", true, "kW");
            drawer.Field("powerCurve");
            drawer.Field("maxLossTorque", true, "Nm");
            drawer.Field("powerModifierSum", false, "x100 %");
            drawer.EndSubsection();

            if (!isElectric)
            {
                drawer.BeginSubsection("Starter");
                drawer.Field("starterActive");
                drawer.Field("starterRunTime", true, "s");
                drawer.Field("starterRPMLimit", true, "rpm");
                drawer.Field("starterTorque", true, "Nm");
                drawer.EndSubsection();
                
                drawer.BeginSubsection("Flying Start");
                drawer.Field("flyingStartEnabled");
                drawer.EndSubsection();

                drawer.BeginSubsection("Idler Circuit");
                drawer.Field("idleRPM");
                drawer.EndSubsection();
            }

            drawer.BeginSubsection("Rev Limiter");
            if (drawer.Field("revLimiterEnabled").boolValue)
            {
                drawer.Field("revLimiterActive", false);
                drawer.Field("revLimiterRPM");
                drawer.Field("revLimiterCutoffDuration");
            }

            drawer.EndSubsection();

            if (!isElectric)
            {
                drawer.BeginSubsection("Forced Induction");
                drawer.Property("forcedInduction");
                drawer.EndSubsection();
            }

            drawer.BeginSubsection("Events");
            drawer.Field("OnStart");
            drawer.Field("OnStop");
            drawer.Field("OnRevLimiter");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}