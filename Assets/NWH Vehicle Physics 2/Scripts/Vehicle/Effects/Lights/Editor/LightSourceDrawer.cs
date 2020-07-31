using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(LightSource))]
    public class LightSourceDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }


            drawer.BeginSubsection("Light Source");

            drawer.Field("type");
            if ((LightSource.LightType) drawer.FindProperty("type").enumValueIndex == LightSource.LightType.Light)
            {
                drawer.Field("light");
            }
            else
            {
                drawer.Field("meshRenderer");
                drawer.Field("rendererMaterialIndex");
                drawer.Field("emissionColor");
                drawer.Info("Make sure to tick 'Emission' under the material settings or otherwise the emissive shader variant will not be included in the build.");
            }

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}