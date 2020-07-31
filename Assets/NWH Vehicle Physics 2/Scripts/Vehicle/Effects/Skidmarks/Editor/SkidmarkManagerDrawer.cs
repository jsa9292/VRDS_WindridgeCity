using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(SkidmarkManager))]
    public class SkidmarkManagerDrawer : ComponentNUIPropertyDrawer
    {
        private float infoHeight;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Geometry");
            drawer.Field("maxMarksPerSection");
            drawer.Field("minDistance", true, "m");
            drawer.Field("groundOffset", true, "m");
            drawer.EndSubsection();

            drawer.BeginSubsection("Appearance");
            drawer.Field("smoothing");
            drawer.Field("globalSkidmarkIntensity");
            drawer.Field("maxSkidmarkAlpha");
            drawer.Field("fadeOverDistance");
            drawer.Field("lowerIntensityThreshold");
            drawer.Info("To change appearance of skidmarks on different surfaces, check GroundDetection preset.");
            drawer.EndSubsection();

            drawer.BeginSubsection("Persistent Skidmarks");
            if (drawer.Field("persistentSkidmarks").boolValue)
            {
                drawer.Field("persistentSkidmarkDestroyDistance", true, "m");
            }

            ;
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}