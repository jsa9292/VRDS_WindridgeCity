using System;
using UnityEngine;

namespace VLB
{
    public static class Utils
    {
        public static float ComputeConeRadiusEnd(float fallOffEnd, float spotAngle)
        {
            return fallOffEnd * Mathf.Tan(spotAngle * Mathf.Deg2Rad * 0.5f);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public static string GetPath(Transform current)
        {
            if (current.parent == null)
                return "/" + current.name;
            return GetPath(current.parent) + "/" + current.name;
        }

        public static T NewWithComponent<T>(string name) where T : Component
        {
            return (new GameObject(name, typeof(T))).GetComponent<T>();
        }

        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            var component = self.GetComponent<T>();
            if (component == null)
                component = self.AddComponent<T>();
            return component;
        }

        public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
        {
            return self.gameObject.GetOrAddComponent<T>();
        }

        /// <summary>
        /// true if the bit field or bit fields that are set in flags are also set in the current instance; otherwise, false.
        /// </summary>
        public static bool HasFlag(this Enum mask, Enum flags) // Same behavior than Enum.HasFlag is .NET 4
        {
#if DEBUG
            if (mask.GetType() != flags.GetType())
                throw new System.ArgumentException(string.Format("The argument type, '{0}', is not the same as the enum type '{1}'.", flags.GetType(), mask.GetType()));
#endif
            return ((int)(IConvertible)mask & (int)(IConvertible)flags) == (int)(IConvertible)flags;
        }

        public static Vector2 xy(this Vector3 aVector) { return new Vector2(aVector.x, aVector.y); }
        public static Vector2 xz(this Vector3 aVector) { return new Vector2(aVector.x, aVector.z); }
        public static Vector2 yz(this Vector3 aVector) { return new Vector2(aVector.y, aVector.z); }
        public static Vector2 yx(this Vector3 aVector) { return new Vector2(aVector.y, aVector.x); }
        public static Vector2 zx(this Vector3 aVector) { return new Vector2(aVector.z, aVector.x); }
        public static Vector2 zy(this Vector3 aVector) { return new Vector2(aVector.z, aVector.y); }

        public static float GetVolumeCubic(this Bounds self) { return self.size.x * self.size.y * self.size.z; }
        public static float GetMaxArea2D(this Bounds self) { return Mathf.Max(Mathf.Max(self.size.x * self.size.y, self.size.y * self.size.z), self.size.x * self.size.z); }

        public static Color Opaque(this Color self) { return new Color(self.r, self.g, self.b, 1f); }

        public static void GizmosDrawPlane(Vector3 normal, Vector3 position, Color color, float size = 1f)
        {
            var v3 = Vector3.Cross(normal, Mathf.Abs(Vector3.Dot(normal, Vector3.forward)) < 0.999f ? Vector3.forward : Vector3.up).normalized * size;
            var corner0 = position + v3;
            var corner2 = position - v3;
            v3 = Quaternion.AngleAxis(90f, normal) * v3;
            var corner1 = position + v3;
            var corner3 = position - v3;

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = color;

            Gizmos.DrawLine(corner0, corner2);
            Gizmos.DrawLine(corner1, corner3);
            Gizmos.DrawLine(corner0, corner1);
            Gizmos.DrawLine(corner1, corner2);
            Gizmos.DrawLine(corner2, corner3);
            Gizmos.DrawLine(corner3, corner0);

            //Gizmos.color = Color.red;
            //Gizmos.DrawRay(position, normal);
        }

        // Plane.Translate is not available in Unity 5
        public static Plane TranslateCustom(this Plane plane, Vector3 translation)
        {
            plane.distance += Vector3.Dot(translation.normalized, plane.normal) * translation.magnitude;
            return plane;
        }

        public static bool IsValid(this Plane plane)
        {
            return plane.normal.sqrMagnitude > 0.5f;
        }

        public static void SetKeywordEnabled(this Material mat, string name, bool enabled)
        {
            if(enabled) mat.EnableKeyword(name);
            else mat.DisableKeyword(name);
        }

        public static void SetShaderKeywordEnabled(string name, bool enabled)
        {
            if (enabled) Shader.EnableKeyword(name);
            else Shader.DisableKeyword(name);
        }

        public static Matrix4x4 SampleInMatrix(this Gradient self, int floatPackingPrecision)
        {
            const int kSamplesCount = 16;
            var mat = new Matrix4x4();
            for (int i = 0; i < kSamplesCount; ++i)
            {
                var color = self.Evaluate(Mathf.Clamp01((float)(i) / (kSamplesCount - 1)));
                mat[i] = color.PackToFloat(floatPackingPrecision);
            }
            return mat;
        }

        public static Color[] SampleInArray(this Gradient self, int samplesCount)
        {
            var array = new Color[samplesCount];
            for (int i = 0; i < samplesCount; ++i)
                array[i] = self.Evaluate(Mathf.Clamp01((float)(i) / (samplesCount - 1)));
            return array;
        }

        static Vector4 Vector4_Floor(Vector4 vec) { return new Vector4(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z), Mathf.Floor(vec.w)); }

        public static float PackToFloat(this Color color, int floatPackingPrecision)
        {
            Vector4 iVal = Vector4_Floor(color * (floatPackingPrecision - 1));

            float output = 0;

            output += iVal.x * floatPackingPrecision * floatPackingPrecision * floatPackingPrecision;
            output += iVal.y * floatPackingPrecision * floatPackingPrecision;
            output += iVal.z * floatPackingPrecision;
            output += iVal.w;

            return output;
        }

        public enum FloatPackingPrecision { High = 64, Low = 8, Undef = 0 }
        static FloatPackingPrecision ms_FloatPackingPrecision = FloatPackingPrecision.Undef;

        // OpenGL ES 2.0 GPU (graphicsShaderLevel = 30) usually have low float precision (16 bits on fragments)
        // So we lower the float packing precision on them (8 seems fine on Adreno (TM) 220, NVIDIA Tegra 3 and on Mali-450 MP)
        // https://docs.unity3d.com/Manual/SL-DataTypesAndPrecision.html
        const int kFloatPackingHighMinShaderLevel = 35;

        public static FloatPackingPrecision GetFloatPackingPrecision()
        {
            if (ms_FloatPackingPrecision == FloatPackingPrecision.Undef)
            {
                ms_FloatPackingPrecision = SystemInfo.graphicsShaderLevel >= kFloatPackingHighMinShaderLevel ? FloatPackingPrecision.High : FloatPackingPrecision.Low;
            }
            return ms_FloatPackingPrecision;
        }

        public static void MarkCurrentSceneDirty()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
#if UNITY_5_3_OR_NEWER
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#else
                UnityEditor.EditorApplication.MarkSceneDirty();
#endif
            }
#endif
        }

        public static void MarkObjectDirty(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorUtility.SetDirty(obj);
            }
#endif
        }

#if UNITY_EDITOR
        public static bool IsEditorCamera(Camera cam)
        {
            var sceneView = UnityEditor.SceneView.currentDrawingSceneView;
            if (sceneView)
            {
                return cam == sceneView.camera;
            }
            return false;
        }

        public static void SetSameSceneVisibilityStatesThan(this GameObject self, GameObject model)
        {
            // SceneVisibilityManager is a feature available from 2019.2, but fixed for transient objects only from 2019.3.14f1
            // https://issuetracker.unity3d.com/issues/toggling-of-picking-and-visibility-flags-of-a-gameobject-is-ignored-when-gameobject-dot-hideflags-is-set-to-hideflags-dot-dontsave
    #if UNITY_2019_3_OR_NEWER
            bool pickingDisabled = UnityEditor.SceneVisibilityManager.instance.IsPickingDisabled(model);
            if (pickingDisabled) UnityEditor.SceneVisibilityManager.instance.DisablePicking(self, true);
            else UnityEditor.SceneVisibilityManager.instance.EnablePicking(self, true);

            bool hidden = UnityEditor.SceneVisibilityManager.instance.IsHidden(model);
            if (hidden) UnityEditor.SceneVisibilityManager.instance.Hide(self, true);
            else UnityEditor.SceneVisibilityManager.instance.Show(self, true);
    #endif
        }
#endif
    }

}
