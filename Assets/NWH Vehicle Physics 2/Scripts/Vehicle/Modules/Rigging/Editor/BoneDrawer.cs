using NWH.NUI;
using NWH.VehiclePhysics2.Modules.Rigging;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Rigging
{
    [CustomPropertyDrawer(typeof(Bone))]
    public class BoneDrawer : NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("thisTransform");
            drawer.Field("targetTransform");
            drawer.Field("stretchToTarget");
            drawer.Field("lookAtTarget");
            if (drawer.Field("doubleSided").boolValue)
            {
                drawer.Field("targetTransformB");
            }

            drawer.EndProperty();
            return true;
        }
    }
}