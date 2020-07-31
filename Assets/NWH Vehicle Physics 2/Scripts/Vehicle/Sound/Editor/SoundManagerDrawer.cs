using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    [CustomPropertyDrawer(typeof(SoundManager))]
    public class SoundManagerDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Master Settings");
            drawer.Field("masterVolume");
            drawer.Field("spatialBlend");
            drawer.Field("mixer");
            drawer.EndSubsection();

            drawer.BeginSubsection("Interior Settings");
            drawer.Field("insideVehicle", false);
            drawer.Field("interiorAttenuation", true, "dB");
            drawer.Field("lowPassFrequency", true, "Hz");
            drawer.Field("lowPassQ");
            drawer.EndSubsection();

            drawer.BeginSubsection("Positional Audio");
            drawer.Info("Go to 'Settings' tab to change component positions.");
            drawer.EndSubsection();

            drawer.BeginSubsection("Components");
            int index = drawer.HorizontalToolbar("soundTab",
                new[]
                {
                    "Engine",
                    "Forced Ind.",
                    "Transmission",
                    "Suspension",
                    "Ground",
                    "Collision",
                    "Brakes",
                    "Blinkers",
                    "Misc"
                });

            switch (index)
            {
                case 0:
                    drawer.Property("engineRunningComponent");
                    drawer.Property("engineStartComponent");
                    drawer.Property("engineFanComponent");
                    break;
                case 1:
                    drawer.Property("turboWhistleComponent");
                    drawer.Property("turboFlutterComponent");
                    break;
                case 2:
                    drawer.Property("transmissionWhineComponent");
                    drawer.Property("gearChangeComponent");
                    break;
                case 3:
                    drawer.Property("suspensionBumpComponent");
                    break;
                case 4:
                    drawer.Property("wheelTireNoiseComponent");
                    drawer.Property("wheelSkidComponent");
                    break;
                case 5:
                    drawer.Property("crashComponent");
                    break;
                case 6:
                    drawer.Property("brakeHissComponent");
                    break;
                case 7:
                    drawer.Property("blinkerComponent");
                    break;
                case 8:
                    drawer.Property("hornComponent");
                    drawer.Property("reverseBeepComponent");
                    break;
                default:
                    drawer.Property("engineRunningComponent");
                    break;
            }

            drawer.EndSubsection();
            drawer.EndProperty();
            return true;
        }
    }
}