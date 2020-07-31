using System;
using UnityEngine;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     ScriptableObject holding friction settings for one surface type.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "NWH Vehicle Physics", menuName = "NWH Vehicle Physics/Friction Preset", order = 1)]
    public class FrictionPreset : ScriptableObject
    {
        public const int LUT_RESOLUTION = 1000;

        /// <summary>
        ///     B, C, D and E parameters of short version of Pacejka's magic formula.
        /// </summary>
        [Tooltip("    B, C, D and E parameters of short version of Pacejka's magic formula.")]
        public Vector4 BCDE;

        public float[] LUT = new float[0];

        [SerializeField]
        private AnimationCurve _curve;

        public AnimationCurve Curve
        {
            get { return _curve; }
        }

        private void Awake()
        {
            GenerateLUT();
        }

        /// <summary>
        ///     Generate Curve from B,C,D and E parameters of Pacejka's simplified magic formula
        /// </summary>
        public void UpdateFrictionCurve()
        {
            _curve = new AnimationCurve();
            Keyframe[] frames = new Keyframe[60];
            int n = frames.Length;
            for (int i = 0; i < n; i++)
            {
                float t = i / 59.0f;
                float v = GetFrictionValue(t, BCDE);
                _curve.AddKey(t, v);
            }

            GenerateLUT();
        }

        public void GenerateLUT()
        {
            LUT = new float[LUT_RESOLUTION];
            for (int i = 0; i < LUT_RESOLUTION; i++)
            {
                float t = i / (float) LUT_RESOLUTION;
                LUT[i] = _curve.Evaluate(t);
            }
        }

        private void OnValidate()
        {
            GenerateLUT();
        }

        private static float GetFrictionValue(float slip, Vector4 p)
        {
            float B = p.x;
            float C = p.y;
            float D = p.z;
            float E = p.w;
            float t = Mathf.Abs(slip);
            return D * Mathf.Sin(C * Mathf.Atan(B * t - E * (B * t - Mathf.Atan(B * t))));
        }
    }
}