using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
    [CustomPropertyDrawer(typeof(TrailerModule))]
    public class TrailerModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Trailer Settings");
            drawer.Field("attachmentPoint");
            drawer.Field("trailerStand");
            drawer.Field("synchronizeGearShifts");
            drawer.EndSubsection();

            drawer.BeginSubsection("Events");
            drawer.Field("onAttach");
            drawer.Field("onDetach");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}