using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(EngineComponent.ForcedInduction))]
    public class ForcedInductionDrawer : PowertrainComponentDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("useForcedInduction");
            drawer.Field("forcedInductionType");
            drawer.Field("powerGainMultiplier");

            EngineComponent.ForcedInduction forcedInduction =
                SerializedPropertyHelper.GetTargetObjectOfProperty(property) as EngineComponent.ForcedInduction;
            if (forcedInduction != null && forcedInduction.forcedInductionType ==
                EngineComponent.ForcedInduction.ForcedInductionType.Turbocharger)
            {
                drawer.Field("spoolUpTime", true, "s");
                drawer.Field("linearity");
            }

            drawer.Field("boost", false);

            drawer.EndProperty();
            return true;
        }
    }
}