using NWH.NUI;
using NWH.VehiclePhysics2.Powertrain;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.GroundDetection
{
    [CustomPropertyDrawer(typeof(GroundDetection))]
    public class GroundDetectionDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("groundDetectionPreset");

            GroundDetectionPreset gdPreset =
                ((GroundDetection) (SerializedPropertyHelper.GetTargetObjectOfProperty(property)
                    as VehicleComponent))?.groundDetectionPreset;

            if (gdPreset != null)
            {
                drawer.EmbeddedObjectEditor(gdPreset, drawer.positionRect);
            }

            drawer.BeginSubsection("Debug Info");
            GroundDetection groundDetection =
                SerializedPropertyHelper.GetTargetObjectOfProperty(drawer.serializedProperty) as GroundDetection;
            if (groundDetection != null && groundDetection.VehicleController != null)
            {
                for (int i = 0; i < groundDetection.VehicleController.powertrain.wheels.Count; i++)
                {
                    WheelComponent wheelComponent = groundDetection.VehicleController.powertrain.wheels[i];
                    if (wheelComponent != null)
                    {
                        drawer.Label($"{wheelComponent.name}: {wheelComponent.surfacePreset?.name} SurfacePreset");
                    }
                }
            }
            else
            {
                drawer.Info("Debug info is available only in play mode.");
            }

            drawer.EndSubsection();


            drawer.EndProperty();
            return true;
        }
    }
}