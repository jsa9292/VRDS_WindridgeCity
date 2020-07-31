using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Powertrain
{
    [CustomPropertyDrawer(typeof(Solver))]
    public class SolverDrawer : NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("physicsQuality");

            drawer.Space();
            if (Application.isPlaying)
            {
                Solver solver = SerializedPropertyHelper.GetTargetObjectOfProperty(property) as Solver;
                drawer.Label($"Simulating {solver.Components.Count} powertrain components.", false, false);
            }

            drawer.EndProperty();
            return true;
        }
    }
}