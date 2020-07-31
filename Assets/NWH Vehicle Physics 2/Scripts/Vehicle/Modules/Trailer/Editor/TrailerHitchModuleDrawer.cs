using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.Modules.Trailer
{
    [CustomPropertyDrawer(typeof(TrailerHitchModule))]
    public class TrailerHitchModuleDrawer : ModuleDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.BeginSubsection("Attachment");
            drawer.Field("attachmentPoint");
            drawer.Field("attachDistanceThreshold");
            drawer.Field("attachOnPlay");
            drawer.Field("detachable");
            drawer.Field("trailerInRange", false);
            drawer.EndSubsection();

            drawer.BeginSubsection("Joint");
            drawer.Field("breakForce");
            drawer.Field("useHingeJoint");
            drawer.EndSubsection();

            drawer.BeginSubsection("Powertrain");
            drawer.Field("noTrailerPowerCoefficient");
            drawer.EndSubsection();

            drawer.BeginSubsection("Events");
            drawer.Field("onTrailerAttach");
            drawer.Field("onTrailerDetach");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}