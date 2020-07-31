using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    [CustomPropertyDrawer(typeof(TransmissionWhineComponent))]
    public class TransmissionWhineComponentDrawer : SoundComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("baseVolume");
            drawer.Field("basePitch");
            drawer.Field("pitchRange");
            drawer.Field("maxSpeed");
            drawer.Field("offThrottleVolumeCoeff");
            drawer.Field("onThrottleVolumeCoeff");
            drawer.Field("smoothing");
            DrawClipsSection(new List<string> {"Whine"});

            drawer.EndProperty();
            return true;
        }
    }
}