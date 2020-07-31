using System.Collections.Generic;
using NWH.VehiclePhysics2.Cameras;
using NWH.VehiclePhysics2.Input;
using NWH.WheelController3D;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
#endif

namespace NWH.VehiclePhysics2.SetupWizard
{
    public class VehicleSetupWizard : MonoBehaviour
    {
        public bool addCamera = true;
        public bool addCharacterEnterExitPoints = true;

        [FormerlySerializedAs("bodyMeshGO")]
        [FormerlySerializedAs("bodyMesh")]
        public bool addCollider;

        public bool addInputProvider = true;
        public GameObject bodyMeshGameObject;
        public List<GameObject> wheelGameObjects = new List<GameObject>();
        private GameObject _cameraParent;
        private GameObject _wheelControllerParent;

        // Group parents
        private GameObject _wheelParent;

        public void RunSetup()
        {
            Debug.Log("======== VEHICLE SETUP START ========");

            if (transform.localScale != Vector3.one)
            {
                Debug.LogWarning(
                    "Scale of a parent object should be [1,1,1] for Rigidbody and VehicleController to function properly.");
                return;
            }

            // Set vehicle tag
            gameObject.tag = "Vehicle";

            // Add body collider
            if (bodyMeshGameObject != null)
            {
                MeshCollider bodyCollider = bodyMeshGameObject.GetComponent<MeshCollider>();
                if (bodyCollider == null)
                {
                    Debug.Log($"Adding MeshCollider to body mesh object {bodyMeshGameObject.name}");

                    // Add mesh collider to body mesh
                    bodyCollider = bodyMeshGameObject.AddComponent<MeshCollider>();
                    bodyCollider.convex = true;

                    // Set body mesh layer to 'Ignore Raycast' to prevent wheels colliding with the vehicle itself.
                    // This is the default value, you can use other layers by changing the Ignore Layer settings under WheelController inspector.
                    Debug.Log(
                        "Setting layer of body collider to default layer 'Ignore Raycast' to prevent wheels from detecting the vehicle itself." +
                        " If you wish to use some other layer check Ignore Layer settings (WheelController inspector).");
                    bodyMeshGameObject.layer = 2;
                }
            }

            // Add rigidbody
            Rigidbody vehicleRigidbody = GetComponent<Rigidbody>();
            if (vehicleRigidbody == null)
            {
                Debug.Log($"Adding Rigidbody to {name}");

                // Add a rigidbody. No need to change rigidbody values as those are set by the VehicleController
                vehicleRigidbody = gameObject.AddComponent<Rigidbody>();
            }

            // Create WheelController GOs and add WheelControllers
            foreach (GameObject wheelObject in wheelGameObjects)
            {
                string objName = $"{wheelObject.name}_WheelController";
                Debug.Log($"Creating new WheelController object {objName}");

                if (!transform.Find(objName))
                {
                    GameObject wcGo = new GameObject(objName);
                    wcGo.transform.SetParent(transform);

                    // Position the WheelController GO to the same position as the wheel
                    wcGo.transform.SetPositionAndRotation(wheelObject.transform.position,
                        wheelObject.transform.rotation);

                    // Move spring anchor to be above the wheel
                    wcGo.transform.position += transform.up * 0.2f;

                    Debug.Log($"   |-> Adding WheelController to {wcGo.name}");

                    // Add WheelController
                    WheelController wheelController = wcGo.AddComponent<WheelController>();

                    // Assign visual to WheelController
                    wheelController.Visual = wheelObject;

                    // Attempt to find radius and width
                    MeshRenderer mr = wheelObject.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        float radius = mr.bounds.extents.y;
                        if (radius < 0.05f || radius > 1f)
                        {
                            Debug.LogWarning(
                                "Detected unusual wheel radius. Adjust WheelController's radius field manually.");
                        }

                        Debug.Log($"   |-> Setting radius to {radius}");
                        wheelController.wheel.radius = radius;

                        float width = mr.bounds.extents.x * 2f;
                        if (width < 0.02f || width > 1f)
                        {
                            Debug.LogWarning(
                                "Detected unusual wheel width. Adjust WheelController's width field manually.");
                        }

                        Debug.Log($"   |-> Setting width to {width}");
                        wheelController.wheel.width = width;
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Radius and width could not be auto configured. Wheel {wheelObject.name} does not contain a MeshFilter.");
                    }
                }
            }

            // Add VehicleController
            VehicleController vehicleController = GetComponent<VehicleController>();
            if (vehicleController == null)
            {
                Debug.Log($"Adding VehicleController to {name}");
                vehicleController = gameObject.AddComponent<VehicleController>();
                vehicleController.SetDefaults();
            }

            // Add camera
            if (addCamera)
            {
                Debug.Log("Adding CameraChanger.");
                GameObject camerasContainer = new GameObject("Cameras");
                camerasContainer.transform.SetParent(transform);
                CameraChanger cameraChanger = camerasContainer.AddComponent<CameraChanger>();

                Debug.Log("Adding a camera follow.");
                GameObject cameraGO = new GameObject("Vehicle Camera");
                cameraGO.transform.SetParent(camerasContainer.transform);
                cameraGO.transform.SetPositionAndRotation(vehicleController.transform.position,
                    vehicleController.transform.rotation);

                Camera camera = cameraGO.AddComponent<Camera>();
                camera.fieldOfView = 80f;

                cameraGO.AddComponent<AudioListener>();

                CameraFollow cameraFollow = cameraGO.AddComponent<CameraFollow>();
                cameraFollow.target = vehicleController;
                cameraFollow.tag = "MainCamera";
            }

            if (addCharacterEnterExitPoints)
            {
                Debug.Log("Adding enter/exit points.");
                GameObject leftPoint = new GameObject("LeftEnterExitPoint");
                GameObject rightPoint = new GameObject("RightEnterExitPoint");

                leftPoint.transform.SetParent(transform);
                rightPoint.transform.SetParent(transform);

                leftPoint.transform.position = transform.position + transform.right;
                rightPoint.transform.position = transform.position - transform.right;

                leftPoint.tag = "EnterExitPoint";
                rightPoint.tag = "EnterExitPoint";
            }

            if (addInputProvider)
            {
                if (FindObjectsOfType<InputProvider>().Length != 0)
                {
                    Debug.LogWarning("InputProvider already present in scene. Skipping.");
                }
                else
                {
                    Debug.Log("Adding input provider (DesktopInputProvider) to object 'VehicleSceneManager'");
                    GameObject managerGO = new GameObject("VehicleSceneManager");
                    managerGO.AddComponent<DesktopInputProvider>();
                }
            }

            // Validate setup
            Debug.Log("Validating setup.");

            // Run Validate() on VehicleController which will report if there are any problems with the setup.
            vehicleController.Validate();

            Debug.Log("Setup done. Removing Wizard.");

            Debug.Log("======== VEHICLE SETUP END ========");

            // Destroy self
            DestroyImmediate(this);
        }
    }
}