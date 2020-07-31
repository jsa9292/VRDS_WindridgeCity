using NWH.NUI;
using NWH.WheelController3D;
using UnityEditor;

namespace NWH.VehiclePhysics2.Tests
{
    [CustomEditor(typeof(VehicleControllerTest))]
    [CanEditMultipleObjects]
    public class VehicleControllerTestEditor : NUIEditor
    {
        private FrictionPreset preset;

        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            VehicleControllerTest test = (VehicleControllerTest) target;
            test.vehicleController = test.GetComponent<VehicleController>();

            if (drawer.Button("Run Tests"))
            {
                test.RunTests();
            }

            if (drawer.Button("Stop Tests"))
            {
                test.StopTests();
            }

            drawer.EndEditor(this);
            return true;
        }
    }
}