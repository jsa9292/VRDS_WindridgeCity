using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2
{
    /// <summary>
    ///     Editor for StateSettings.
    /// </summary>
    [CustomEditor(typeof(StateSettings))]
    [CanEditMultipleObjects]
    public class StateSettingsEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            StateSettings stateSettings = target as StateSettings;


            drawer.BeginSubsection("LODs");
            drawer.Info("LODs must be ordered from lowest to highest distance.");
            drawer.ReorderableList("LODs", "LODs");
            drawer.EndSubsection();

            drawer.BeginSubsection("Component State Definitions");
            drawer.Info(
                "VehicleComponent state definitions are fetched on Awake() and any changes made to them will only " +
                "affect vehicles initialized after the change was made.", MessageType.Warning);
            DrawDefinitionsList(stateSettings);
            drawer.EndSubsection();

            drawer.EndEditor();
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        private void DrawDefinitionsList(StateSettings stateSettings)
        {
            drawer.Label("Vehicle Components:");

            int n = stateSettings.definitions.Count;
            SerializedProperty listProperty = drawer.FindProperty("definitions");
            for (int i = 0; i < n; i++)
            {
                SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
                drawer.Property(element);
            }

            drawer.Space(10);

            if (drawer.Button("Refresh"))
            {
                stateSettings.Reload();
            }
        }
    }
}