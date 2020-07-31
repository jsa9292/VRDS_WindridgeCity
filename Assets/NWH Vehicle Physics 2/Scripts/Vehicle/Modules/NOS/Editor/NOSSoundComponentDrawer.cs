using System.Collections.Generic;
using NWH.VehiclePhysics2.Sound.SoundComponents;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.NOS
{
    [CustomPropertyDrawer(typeof(NOSSoundComponent))]
    public class NOSSoundComponentDrawer : SoundComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("baseVolume");
            drawer.Field("basePitch");
            DrawClipsSection(new List<string> {"Start"});

            drawer.EndProperty();
            return true;
        }
    }
}