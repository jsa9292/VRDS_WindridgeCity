using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Metrics
{
    [CustomPropertyDrawer(typeof(MetricsModule))]
    public class MetricsModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            MetricsModule metrics = SerializedPropertyHelper.GetTargetObjectOfProperty(property) as MetricsModule;
            if (metrics == null)
            {
                drawer.EndProperty();
                return false;
            }

            drawer.Label($"Top Speed: {metrics.topSpeed.value}");
            drawer.Label($"Average Speed: {metrics.averageSpeed.value}");
            drawer.Label($"Odometer: {metrics.odometer.value}");
            drawer.Label($"Cont. Drift Distance: {metrics.continousDriftDistance.value}");
            drawer.Label($"Cont. Drift Time: {metrics.continousDriftTime.value}");
            drawer.Label($"Total Drift Distance: {metrics.totalDriftDistance.value}");
            drawer.Label($"Total Drift Time: {metrics.totalDriftTime.value}");

            drawer.EndProperty();
            return true;
        }
    }
}