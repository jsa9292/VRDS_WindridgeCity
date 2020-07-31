using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.VehiclePhysics2.SceneManagement
{
    [CustomEditor(typeof(VehicleChanger))]
    [CanEditMultipleObjects]
    public class VehicleChangerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            VehicleChanger sc = drawer.GetObject<VehicleChanger>();

            drawer.BeginSubsection("Vehicles");
            drawer.ReorderableList("vehicles");
            drawer.Field("vehicleTag");
            if (drawer.Field("characterBased").boolValue)
            {
                drawer.Info(
                    "When 'Character Based' is set to true make sure that you have CharacterVehicleChanger present in the scene.");
            }

            drawer.Field("deactivateAll");
            drawer.Field("putOtherVehiclesToSleep");
            drawer.Field("currentVehicleIndex");
            if (Application.isPlaying)
            {
                drawer.Label("Active Vehicle: " + VehicleChanger.ActiveVehicleController);
            }

            if (drawer.Button("Next Vehicle"))
            {
                sc.NextVehicle();
            }

            if (drawer.Button("Previous Vehicle"))
            {
                sc.PreviousVehicle();
            }

            drawer.EndSubsection();

            drawer.EndSubsection();

            drawer.EndEditor(this);
            return true;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}