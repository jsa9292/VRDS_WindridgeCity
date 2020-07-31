using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(Powertrain))]
    public class PowertrainDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            int powertrainTab = drawer.HorizontalToolbar("powertrainTab",
                new[] {"Solver", "Engine", "Clutch", "Transmission", "Differentials", "Wheels", "Wheel Groups"});

            switch (powertrainTab)
            {
                case 0:
                    drawer.Property("solver");
                    break;
                case 1:
                    drawer.Property("engine");
                    break;
                case 2:
                    drawer.Property("clutch");
                    break;
                case 3:
                    drawer.Property("transmission");
                    break;
                case 4:
                    drawer.ReorderableList("differentials", null, true, true, null, 5f);
                    break;
                case 5:
                    drawer.ReorderableList("wheels", null, true, true, null, 5f);
                    break;
                case 6:
                    drawer.ReorderableList("wheelGroups", null, true, true, null, 5f);
                    break;
            }

            drawer.EndProperty();
            return true;
        }
    }
}