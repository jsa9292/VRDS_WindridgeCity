using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Property drawer for Input.
    /// </summary>
    [CustomPropertyDrawer(typeof(Input))]
    public class InputDrawer : ComponentNUIPropertyDrawer
    {
        private float infoHeight;

        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("throttleType");
            drawer.Field("autoSettable");
            drawer.Field("states");

            drawer.EndProperty();
            return true;
        }
    }
}