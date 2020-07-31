using NWH.NUI;
using UnityEditor;

namespace NWH.VehiclePhysics2.SceneManagement
{
    [CustomEditor(typeof(CharacterVehicleChanger))]
    [CanEditMultipleObjects]
    public class CharacterVehicleChangerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            CharacterVehicleChanger cvc = drawer.GetObject<CharacterVehicleChanger>();

            drawer.Field("characterObject");
            drawer.Field("enterDistance", true, "m");
            drawer.Field("enterExitTag");
            drawer.Field("maxEnterExitVehicleSpeed", true, "m/s");
            drawer.Field("nearVehicle", false);

            if (drawer.Button("Enter/Exit Vehicle"))
            {
                cvc.EnterExitVehicle();
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