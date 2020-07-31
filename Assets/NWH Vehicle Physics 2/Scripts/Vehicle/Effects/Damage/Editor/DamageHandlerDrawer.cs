using NWH.NUI;
using NWH.VehiclePhysics2.Powertrain;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Effects
{
    [CustomPropertyDrawer(typeof(DamageHandler))]
    public class DamageHandlerDrawer : ComponentNUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            DamageHandler damageHandler = SerializedPropertyHelper.GetTargetObjectOfProperty(property) as DamageHandler;
            if (damageHandler == null)
            {
                drawer.EndProperty();
                return false;
            }

            drawer.BeginSubsection("Collision");
            drawer.Field("decelerationThreshold", true, "m/s2");
            drawer.Field("collisionTimeout", true, "s");
            drawer.ReorderableList("collisionIgnoreTags");
            drawer.EndSubsection();

            drawer.BeginSubsection("Damage");
            drawer.Field("damageIntensity");
            if (damageHandler.VehicleController != null)
            {
                drawer.Label($"Current Damage: {damageHandler.Damage} ({damageHandler.Damage * 100f}%)");
                drawer.Label($"Engine Damage: {damageHandler.VehicleController.powertrain.engine.ComponentDamage}");
                drawer.Label(
                    $"Transmission Damage: {damageHandler.VehicleController.powertrain.transmission.ComponentDamage}");
                foreach (WheelComponent wheelComponent in damageHandler.VehicleController.Wheels)
                {
                    drawer.Label($"Wheel {wheelComponent.wheelController.name} Damage: {wheelComponent.Damage}");
                }
            }
            else
            {
                drawer.Info("Damage debug info available in play mode.");
            }

            drawer.EndSubsection();

            drawer.BeginSubsection("Mesh Deformation");
            if (drawer.Field("meshDeform").boolValue)
            {
                drawer.Field("deformationVerticesPerFrame");
                drawer.Field("deformationRadius", true, "m");
                drawer.Field("deformationStrength");
                drawer.Field("deformationRandomness");
                drawer.ReorderableList("deformationIgnoreTags");
            }

            drawer.EndSubsection();

            drawer.BeginSubsection("Actions");
            if (drawer.Button("Repair"))
            {
                damageHandler.Repair();
            }

            drawer.EndSubsection();

            drawer.BeginSubsection("Events");
            drawer.Field("OnCollision");
            drawer.EndSubsection();


            drawer.EndProperty();
            return true;
        }
    }
}