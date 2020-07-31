using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.SetupWizard
{
    [CustomEditor(typeof(VehicleSetupWizard))]
    [CanEditMultipleObjects]
    public class VehicleSetupWizardEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            VehicleSetupWizard sw = drawer.GetObject<VehicleSetupWizard>();
            if (sw == null)
            {
                return false;
            }

            if (PrefabUtility.GetPrefabInstanceStatus(sw.gameObject) == PrefabInstanceStatus.Connected)
            {
                drawer.Info("Setup Wizard should not be run in prefab mode, only scene mode.", MessageType.Warning);
            }

            drawer.BeginSubsection("Collider");
            if (drawer.Field("addCollider").boolValue)
            {
                drawer.Field("bodyMeshGameObject");
            }
            else
            {
                drawer.Info("When setting up collider(s) manually make sure that all colliders are either on" +
                            " 'Physics.IgnoreRaycast' or one of the WheelController's ignore layers.");
            }

            drawer.EndSubsection();

            drawer.BeginSubsection("Wheels");
            drawer.Info(
                "Wheel GameObjects should be added in the left-right, front-to-back order e.g. frontLeft, frontRight, rearLeft, rearRight");
            drawer.ReorderableList("wheelGameObjects");
            drawer.EndSubsection();

            drawer.BeginSubsection("Options");
            drawer.Field("addCamera");
            drawer.Field("addCharacterEnterExitPoints");
            drawer.Field("addInputProvider");

            drawer.EndSubsection();

            drawer.HorizontalRuler();

            if (drawer.Button("Run Setup"))
            {
                sw.RunSetup();
            }

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}