using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    [CustomPropertyDrawer(typeof(ReverseBeepComponent))]
    public class ReverseBeepComponentDrawer : SoundComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("baseVolume");
            drawer.Field("basePitch");
            drawer.Field("beepOnReverseGear");
            drawer.Field("beepOnNegativeVelocity");
            DrawClipsSection(new List<string> {"Beep"});

            drawer.EndProperty();
            return true;
        }
    }
}