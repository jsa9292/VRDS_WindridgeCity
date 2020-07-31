using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Fuel
{
    [CustomPropertyDrawer(typeof(FuelModule))]
    public class FuelModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Settings");
            drawer.Field("capacity", true, "L");
            drawer.Field("amount", true, "L");
            drawer.Field("efficiency");
            drawer.Field("idleConsumption");
            drawer.Field("consumptionMultiplier");
            drawer.EndSubsection();

            drawer.BeginSubsection("Debug");
            drawer.Field("consumptionLPer100km", false);
            drawer.Field("consumptionPerHour", false);
            drawer.EndSubsection();

            drawer.BeginSubsection("Events");
            drawer.Field("onOutOfFuel");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}