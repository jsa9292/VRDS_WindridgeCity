using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules
{
    [CustomEditor(typeof(ModuleWrapper), true)]
    [CanEditMultipleObjects]
    public class ModuleWrapperEditor : NUIEditor
    {
        public override void OnInspectorGUI()
        {
            //OnInspectorNUI();
            EditorGUILayout.HelpBox(
                new GUIContent("To change module settings go to 'Modules' tab of VehicleController."));
        }

        public override bool OnInspectorNUI()
        {
            drawer.BeginEditor(serializedObject);
            drawer.Property(drawer.serializedObject.FindProperty("module"));
            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}