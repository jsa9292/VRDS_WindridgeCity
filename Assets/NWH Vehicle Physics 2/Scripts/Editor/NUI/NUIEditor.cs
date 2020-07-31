using UnityEditor;

namespace NWH.NUI
{
    public class NUIEditor : Editor
    {
        public NUIDrawer drawer = new NUIDrawer();

        public override void OnInspectorGUI()
        {
            OnInspectorNUI();
        }

        public virtual bool OnInspectorNUI()
        {
            if (drawer == null)
            {
                drawer = new NUIDrawer();
            }

            drawer.BeginEditor(serializedObject);
            if (!drawer.Header(serializedObject.targetObject.GetType().Name))
            {
                drawer.EndEditor();
                return false;
            }

            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}