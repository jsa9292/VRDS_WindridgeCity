using System.Collections.Generic;
using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.Sound.SoundComponents
{
    [CustomPropertyDrawer(typeof(SoundComponent), true)]
    public class SoundComponentDrawer : ComponentNUIPropertyDrawer
    {
        public SoundComponent soundComponentObject;

        public void DrawClipsSection(List<string> clipNames = null, int minClipCount = 1,
            int maxClipCount = 1, int defaultClipCount = 1)
        {
            soundComponentObject = (SerializedPropertyHelper.GetTargetObjectOfProperty(drawer.serializedProperty)
                as VehicleComponent) as SoundComponent;
            drawer.ReorderableList("clips");

            if (soundComponentObject == null)
            {
                return;
            }

            if (soundComponentObject.clips.Count > defaultClipCount)
            {
                drawer.Space();
                drawer.Info("If more than clip is supplied for sound components that require only one, " +
                            "a random clip will be selected each time sound is played.");
            }
        }
    }
}