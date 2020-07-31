using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(EffectManager))]
    public class EffectManagerDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            int effectsTab = drawer.HorizontalToolbar("effectsTab",
                new[] {"Skidmarks", "Lights", "Surf. Part.", "Ex. Smoke", "Ex. Flash"});

            switch (effectsTab)
            {
                case 0:
                    drawer.Property("skidmarkManager");
                    break;
                case 1:
                    drawer.Property("lightsManager");
                    break;
                case 2:
                    drawer.Property("surfaceParticleManager");
                    break;
                case 3:
                    drawer.Property("exhaustSmoke");
                    break;
                case 4:
                    drawer.Property("exhaustFlash");
                    break;
                default:
                    drawer.Property("skidmarks");
                    break;
            }


            drawer.EndProperty();
            return true;
        }
    }
}