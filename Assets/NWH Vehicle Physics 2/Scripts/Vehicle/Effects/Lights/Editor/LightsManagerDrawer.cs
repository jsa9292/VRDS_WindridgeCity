using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(LightsMananger))]
    public class LightsManagerDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            int menuIndex =
                drawer.HorizontalToolbar("LightsMenu", new[] {"Front", "Rear", "Blinkers", "Misc"}, true, true);

            if (menuIndex == 0)
            {
                drawer.Field("lowBeamLights");
                drawer.Field("highBeamLights");
            }
            else if (menuIndex == 1)
            {
                drawer.Field("tailLights");
                drawer.Field("brakeLights");
                drawer.Field("reverseLights");
            }
            else if (menuIndex == 2)
            {
                drawer.Field("leftBlinkers");
                drawer.Field("rightBlinkers");
            }
            else
            {
                drawer.Field("extraLights");
            }

            drawer.EndProperty();
            return true;
        }
    }
}