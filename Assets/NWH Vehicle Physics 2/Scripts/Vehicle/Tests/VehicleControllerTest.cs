using System.Collections.Generic;
using UnityEngine;

namespace NWH.VehiclePhysics2.Tests
{
    [RequireComponent(typeof(VehicleController))]
    public class VehicleControllerTest : MonoBehaviour
    {
        public VehicleController vehicleController;
        private List<VehicleComponent> components = new List<VehicleComponent>();

        private void Awake()
        {
            vehicleController = GetComponent<VehicleController>();
        }

        private void RandomlyEnableDisableComponent()
        {
            int randomIndex = Random.Range(0, components.Count);
            bool enable = Random.Range(0f, 1f) > 0.5f;
            VehicleComponent component = components[randomIndex];

            if (enable)
            {
                Debug.Log($"Enable {component.GetType().Name}");
                components[randomIndex].Enable();
                Debug.Assert(component.IsEnabled || !component.IsOn);
            }
            else
            {
                Debug.Log($"Disable {component.GetType().Name}");
                components[randomIndex].Disable();
                Debug.Assert(!component.IsEnabled);
            }
        }

        public void RunStateTest()
        {
            components = new List<VehicleComponent>();

            // Add main
            components.Add(vehicleController.steering);
            components.Add(vehicleController.powertrain);
            components.Add(vehicleController.damageHandler);
            components.Add(vehicleController.brakes);
            components.Add(vehicleController.groundDetection);
            components.Add(vehicleController.moduleManager);

            // Add effects
            components.Add(vehicleController.effectsManager);
            components.AddRange(vehicleController.effectsManager.components);

            // Add sound
            components.Add(vehicleController.soundManager);
            components.AddRange(vehicleController.soundManager.components);

            InvokeRepeating("RandomlyEnableDisableComponent", 0.1f, 0.02f);
        }

        public void RunTests()
        {
            RunStateTest();
        }

        public void StopTests()
        {
            CancelInvoke("RandomlyEnableDisableComponent");
        }
    }
}