using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NWH.NUI
{
    public static class EditorCache
    {
        // Store data for each property as property drawer gets reused multiple times and local values overwritten
        private static readonly Dictionary<string, dynamic> Cache = new Dictionary<string, dynamic>
        {
            {"height", new Dictionary<string, float>()},
            {"ReorderableList", new Dictionary<string, ReorderableList>()},
            {"guiWasEnabled", new Dictionary<string, bool>()},
            {"NUIEditor", new Dictionary<string, NUIEditor>()},
            {"isExpanded", new Dictionary<string, bool>()},
            {"tabIndex", new Dictionary<string, int>()},
            {"Texture2D", new Dictionary<string, Texture2D>()},
            {"SerializedProperty", new Dictionary<string, SerializedProperty>()}
        };

        public static bool GetCachedValue<T>(string variableName, ref T value, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (!Cache.ContainsKey(variableName) || !Cache[variableName].ContainsKey(key))
            {
                return false;
            }

            value = Cache[variableName][key];
            return true;
        }

        public static bool SetCachedValue<T>(string variableName, T value, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (Cache.ContainsKey(variableName))
            {
                if (!Cache[variableName].ContainsKey(key))
                {
                    Cache[variableName].Add(key, value);
                }
                else
                {
                    Cache[variableName][key] = value;
                }

                return true;
            }

            return false;
        }
    }
}