using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    [CustomPropertyDrawer(typeof(SuspensionBumpComponent))]
    public class SuspensionBumpComponentDrawer : SoundComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("baseVolume");
            drawer.Field("basePitch");
            DrawClipsSection(new List<string> {"Bump"}, 1, 10);

            drawer.EndProperty();
            return true;
        }
    }
}