#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace VLB
{
    [CustomEditor(typeof(ConfigOverride))]
    public class ConfigOverrideEditor : ConfigEditor
    {
        protected override bool IsOverriddenInstance() { return true; }

        public static void CreateAsset()
        {
            const string kFolderParent = "Assets";
            const string kFolderResources = "Resources";

            var asset = CreateInstance<ConfigOverride>();

            if (!AssetDatabase.IsValidFolder(string.Format("{0}/{1}", kFolderParent, kFolderResources)))
                AssetDatabase.CreateFolder(kFolderParent, kFolderResources);

            AssetDatabase.CreateAsset(asset, string.Format("{0}/{1}/{2}.asset", kFolderParent, kFolderResources, ConfigOverride.kAssetName));
            AssetDatabase.SaveAssets();

            Config.EditorSelectInstance();
        }
    }
}
#endif
