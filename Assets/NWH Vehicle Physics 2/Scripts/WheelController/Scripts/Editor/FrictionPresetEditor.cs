using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Editor for FrictionPreset.
    /// </summary>
    [CustomEditor(typeof(FrictionPreset))]
    [CanEditMultipleObjects]
    public class FrictionPresetEditor : NUIEditor
    {
        private FrictionPreset preset;

        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            preset = (FrictionPreset) target;
            Vector4 initialBCDE = preset.BCDE;
            float B = preset.BCDE.x;
            float C = preset.BCDE.y;
            float D = preset.BCDE.z;
            float E = preset.BCDE.w;

            drawer.BeginSubsection("Pacejka Parameters");

            drawer.SplitRectVertically(drawer.positionRect, 0.2f, out Rect labelRect, out Rect valueRect);
            EditorGUI.LabelField(labelRect, "B");
            B = EditorGUI.Slider(valueRect, B, 0, 30);
            drawer.AdvancePosition();

            drawer.SplitRectVertically(drawer.positionRect, 0.2f, out labelRect, out valueRect);
            EditorGUI.LabelField(labelRect, "C");
            C = EditorGUI.Slider(valueRect, C, 0, 5);
            drawer.AdvancePosition();

            drawer.SplitRectVertically(drawer.positionRect, 0.2f, out labelRect, out valueRect);
            EditorGUI.LabelField(labelRect, "D");
            D = EditorGUI.Slider(valueRect, D, 0, 2);
            drawer.AdvancePosition();

            drawer.SplitRectVertically(drawer.positionRect, 0.2f, out labelRect, out valueRect);
            EditorGUI.LabelField(labelRect, "E");
            E = EditorGUI.Slider(valueRect, E, 0, 2);
            drawer.AdvancePosition();

            drawer.EndSubsection();

            drawer.BeginSubsection("Friction Curve Preview");
            Rect curveRect = new Rect(drawer.positionRect.x, drawer.positionRect.y, drawer.positionRect.width, 90f);
            EditorGUI.CurveField(curveRect, preset.Curve);
            drawer.AdvancePosition(92f);
            drawer.Info("X: Slip | Y: Friction");
            drawer.EndSubsection();

            preset.BCDE = new Vector4(B, C, D, E);

            if (preset.BCDE != initialBCDE)
            {
                preset.UpdateFrictionCurve();
            }

            drawer.EndEditor(this);
            return true;
        }
    }
}