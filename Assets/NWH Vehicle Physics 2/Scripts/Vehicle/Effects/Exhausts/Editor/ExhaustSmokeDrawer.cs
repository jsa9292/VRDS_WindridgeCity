using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(ExhaustSmoke))]
    public class ExhaustSmokeDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Particle Settings");
            drawer.Field("normalColor");
            drawer.Field("sootColor");
            drawer.Field("sootIntensity");
            drawer.Field("lifetimeDistance");
            drawer.Field("maxSizeMultiplier");
            drawer.Field("maxSpeedMultiplier");
            drawer.ReorderableList("particleSystems");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}