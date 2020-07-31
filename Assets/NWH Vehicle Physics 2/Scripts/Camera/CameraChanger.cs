using System.Collections.Generic;
using System.Linq;
using NWH.VehiclePhysics2.Input;
using UnityEngine;

namespace NWH.VehiclePhysics2.Cameras
{
    /// <summary>
    ///     Switches between the camera objects that are children to this object and contain camera tag,
    ///     in order they appear in the hierarchy or in order they are added to the vehicle cameras list.
    /// </summary>
    public class CameraChanger : MonoBehaviour
    {
        /// <summary>
        ///     If true vehicleCameras list will be filled through cameraTag.
        /// </summary>
        [Tooltip("    If true vehicleCameras list will be filled through cameraTag.")]
        public bool autoFindCameras = true;

        /// <summary>
        ///     Index of the camera from vehicle cameras list that will be active first.
        /// </summary>
        [Tooltip("    Index of the camera from vehicle cameras list that will be active first.")]
        public int currentCameraIndex;

        /// <summary>
        ///     List of cameras that the changer will cycle through. Leave empty if you want cameras to be automatically detected.
        ///     To be detected cameras need to have camera tag and be children of the object this script is attached to.
        /// </summary>
        [Tooltip(
            "List of cameras that the changer will cycle through. Leave empty if you want cameras to be automatically detected." +
            " To be detected cameras need to have camera tag and be children of the object this script is attached to.")]
        public List<GameObject> vehicleCameras = new List<GameObject>();

        private CameraInsideVehicle _cis;
        private int _previousCamera;
        private VehicleController _vehicleController;

        private void Awake()
        {
            _vehicleController = GetComponentInParent<VehicleController>();
            if (_vehicleController == null)
            {
                Debug.LogError("None of the parent objects of CameraChanger contain VehicleController.");
            }

            _vehicleController.onWake.AddListener(OnVehicleWake);
            _vehicleController.onSleep.AddListener(OnVehicleSleep);

            if (_vehicleController == null)
            {
                Debug.Log("None of the parents of camera changer contain VehicleController component. " +
                          "Make sure that the camera changer is amongst the children of VehicleController object.");
            }

            if (autoFindCameras)
            {
                vehicleCameras = new List<GameObject>();
                foreach (Camera cam in GetComponentsInChildren<Camera>(true))
                {
                    vehicleCameras.Add(cam.gameObject);
                }
            }

            if (vehicleCameras.Count == 0)
            {
                Debug.LogWarning("No cameras could. Either add cameras manually or " +
                                 "add them as children to the game object this script is attached to.");
            }

            if (!_vehicleController.IsAwake ||
                _vehicleController.multiplayerInstanceType == VehicleController.MultiplayerInstanceType.Remote)
            {
                DisableAllCameras();
            }
            else
            {
                EnableCurrentDisableOthers();
                CheckIfInside();
            }
        }


        private void Update()
        {
            if (_vehicleController.IsAwake &&
                _vehicleController.multiplayerInstanceType == VehicleController.MultiplayerInstanceType.Local &&
                InputProvider.Instances.Count > 0)
            {
                bool changeCamera = false;
                foreach (InputProvider i in InputProvider.Instances)
                {
                    if (i.ChangeCamera())
                    {
                        changeCamera = true;
                        break;
                    }
                }

                if (changeCamera)
                {
                    NextCamera();
                    CheckIfInside();
                }
            }
        }

        private void EnableCurrentDisableOthers()
        {
            int cameraCount = vehicleCameras.Count;
            for (int i = 0; i < cameraCount; i++)
            {
                if (vehicleCameras[i] == null)
                {
                    continue;
                }

                if (i == currentCameraIndex)
                {
                    vehicleCameras[i].SetActive(true);
                    AudioListener al = vehicleCameras[i].GetComponent<AudioListener>();
                    if (al != null)
                    {
                        al.enabled = true;
                    }
                }
                else
                {
                    vehicleCameras[i].SetActive(false);
                    AudioListener al = vehicleCameras[i].GetComponent<AudioListener>();
                    if (al != null)
                    {
                        al.enabled = false;
                    }
                }
            }
        }

        private void DisableAllCameras()
        {
            int cameraCount = vehicleCameras.Count;
            for (int i = 0; i < cameraCount; i++)
            {
                vehicleCameras[i].SetActive(false);
                AudioListener al = vehicleCameras[i].GetComponent<AudioListener>();
                if (al != null)
                {
                    al.enabled = true;
                }
            }
        }

        /// <summary>
        ///     Activates next camera in order the camera scripts are attached to the camera object.
        /// </summary>
        public void NextCamera()
        {
            if (vehicleCameras.Count <= 0)
            {
                return;
            }

            currentCameraIndex++;
            if (currentCameraIndex >= vehicleCameras.Count)
            {
                currentCameraIndex = 0;
            }

            EnableCurrentDisableOthers();
        }

        public void PreviousCamera()
        {
            if (vehicleCameras.Count <= 0)
            {
                return;
            }

            currentCameraIndex--;
            if (currentCameraIndex < 0)
            {
                currentCameraIndex = vehicleCameras.Count - 1;
            }

            EnableCurrentDisableOthers();
        }


        private void CheckIfInside()
        {
            if (vehicleCameras.Count == 0 || vehicleCameras[currentCameraIndex] == null)
            {
                return;
            }

            _cis = vehicleCameras[currentCameraIndex].gameObject.GetComponent<CameraInsideVehicle>();

            if (_cis != null && _cis.isInsideVehicle)
            {
                _vehicleController.soundManager.insideVehicle = true;
            }
            else
            {
                _vehicleController.soundManager.insideVehicle = false;
            }
        }

        private void OnVehicleWake()
        {
            EnableCurrentDisableOthers();
        }

        private void OnVehicleSleep()
        {
            DisableAllCameras();
        }
    }
}