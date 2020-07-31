#if UNITY_EDITOR
#if UNITY_2019_1_OR_NEWER
#define UI_USE_FOLDOUT_HEADER_2019
#endif

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

namespace VLB
{
    public class EditorCommon : Editor
    {
        protected virtual void OnEnable()
        {
            FoldableHeader.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
    #if UNITY_2019_3_OR_NEWER
            // no vertical space in 2019.3 looks better
    #else
            EditorGUILayout.Separator();
    #endif
        }

        public static void DrawLineSeparator()
        {
            DrawLineSeparator(Color.grey, 1, 10);
        }

        static void DrawLineSeparator(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));

            r.x = 0;
            r.width = EditorGUIUtility.currentViewWidth;

            r.y += padding / 2;
            r.height = thickness;

            EditorGUI.DrawRect(r, color);
        }

        protected SerializedProperty FindProperty<T, TValue>(Expression<Func<T, TValue>> expr)
        {
            Debug.Assert(serializedObject != null);
            return serializedObject.FindProperty(ReflectionUtils.GetFieldPath(expr));
        }

        protected void ButtonOpenConfig(bool miniButton = true)
        {
            bool buttonClicked = false;
            if (miniButton) buttonClicked = GUILayout.Button(EditorStrings.Common.ButtonOpenGlobalConfig, EditorStyles.miniButton);
            else            buttonClicked = GUILayout.Button(EditorStrings.Common.ButtonOpenGlobalConfig);

            if (buttonClicked)
                Config.EditorSelectInstance();
        }
    }
}
#endif // UNITY_EDITOR

