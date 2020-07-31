using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VLB
{
    public class MaterialKeywordSRP
    {
        public const string kKeyword = "VLB_SRP_API";

#if UNITY_EDITOR
        static string GetPath(Shader shader)
        {
            const string kDummyFilename = "DummyMaterial.mat";
            const string kDummyPathFallback = "Assets/VolumetricLightBeam/Resources/" + kDummyFilename;

            if (shader == null)
                return kDummyPathFallback;

            var shaderPath = AssetDatabase.GetAssetPath(shader);
            if (string.IsNullOrEmpty(shaderPath))
                return kDummyPathFallback;

            const string kFolderPattern = "Resources/";
            var index = shaderPath.LastIndexOf(kFolderPattern);
            if (index < 0)
                return kDummyPathFallback;

            index += kFolderPattern.Length;
            if (index >= shaderPath.Length)
                return kDummyPathFallback;

            return shaderPath.Substring(0, index) + kDummyFilename;
        }

        /// <summary>
        /// Create a dummy material with the VLB_SRP_API keyword enabled placed in a Resource folder
        /// to prevent from stripping away this shader variant when exporting a build.
        /// </summary>
        public static void Create(Shader shader)
        {
            if (shader == null)
                return;

            string path = GetPath(shader);
            var dummyMat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (dummyMat == null || dummyMat.shader != shader)
            {
                dummyMat = new Material(shader);
                if (dummyMat)
                    AssetDatabase.CreateAsset(dummyMat, path);
            }

            if (dummyMat.IsKeywordEnabled(kKeyword) == false)
            {
                dummyMat.EnableKeyword(kKeyword);
                EditorUtility.SetDirty(dummyMat);
            }
        }

        public static void Delete(Shader shader)
        {
            string path = GetPath(shader);
            var dummyMat = AssetDatabase.LoadAssetAtPath<Material>(path); // make sure the asset exists before deleting it, otherwise it can raise exceptions in specific conditions
            if (dummyMat != null)
                AssetDatabase.DeleteAsset(path);
        }
#endif
    }
}