using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    [CustomPropertyDrawer(typeof(WheelTireNoiseComponent))]
    public class WheelTireNoiseComponentDrawer : SoundComponentDrawer
    {
        private float infoHeight;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Info("Check SurfaceMaps to change per-surface clips and settings.");

            drawer.EndProperty();
            return true;
        }
    }
}