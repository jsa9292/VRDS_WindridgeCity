using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    [CustomPropertyDrawer(typeof(EngineRunningComponent))]
    public class EngineRunningComponentDrawer : SoundComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("baseVolume");
            drawer.Field("basePitch");
            drawer.Field("volumeRange");
            drawer.Field("pitchRange");
            drawer.Field("smoothing");
            drawer.Field("maxDistortion");

            DrawClipsSection(new List<string> {"Running"});

            drawer.EndProperty();
            return true;
        }
    }
}