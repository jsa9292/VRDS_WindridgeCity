using NWH.VehiclePhysics2.Cameras;
using NWH.VehiclePhysics2.SceneManagement;
using UnityEngine;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Class for handling mobile user input via touch screen and sensors.
    /// </summary>
    public class MobileInputProvider : InputProvider
    {
        /// <summary>
        ///     Steer input device.
        ///     Accelerometer - uses sensors to get horizontal axis.
        ///     Steering Wheel - uses SteeringWheel script and steering wheel on-screen graphic that can be rotated by dragging.
        ///     Button - uses button to get horizontal axis.
        /// </summary>
        public enum HorizontalAxisType
        {
            Accelerometer,
            SteeringWheel,
            Button,
            Screen
        }

        public enum VerticalAxisType
        {
            Accelerometer,
            Button,
            Screen
        }

        public MobileInputButton boostButton;
        public MobileInputButton brakeButton;
        public MobileInputButton changeCameraButton;
        public MobileInputButton changeVehicleButton;
        public MobileInputButton cruiseControlButton;
        public MobileInputButton engineStartStopButton;
        public MobileInputButton extraLightsButton;
        public MobileInputButton flipOverButton;
        public MobileInputButton handbrakeButton;
        public MobileInputButton hazardLightsButton;
        public MobileInputButton highBeamLightsButton;

        /// <summary>
        ///     Active steer devices.
        /// </summary>
        [Tooltip("    Active steer devices.")]
        public HorizontalAxisType horizontalInputType = HorizontalAxisType.SteeringWheel;

        public MobileInputButton hornButton;
        public MobileInputButton leftBlinkerButton;
        public MobileInputButton lowBeamLightsButton;
        public MobileInputButton rightBlinkerButton;
        public MobileInputButton shiftDownButton;
        public MobileInputButton shiftUpButton;

        /// <summary>
        ///     Steering wheel script. Optional and not needed if SteeringWheel option is not used.
        /// </summary>
        [Tooltip("    Steering wheel script. Optional and not needed if SteeringWheel option is not used.")]
        public SteeringWheel steeringWheel;

        public MobileInputButton steerLeftButton;
        public MobileInputButton steerRightButton;

        public MobileInputButton throttleButton;

        /// <summary>
        ///     Higher value will result in higher steer angle for same tilt.
        /// </summary>
        [Tooltip("    Higher value will result in higher steer angle for same tilt.")]
        public float tiltSensitivity = 1.5f;

        public MobileInputButton trailerAttachDetachButton;

        /// <summary>
        ///     Active steer devices.
        /// </summary>
        [Tooltip("    Active steer devices.")]
        public VerticalAxisType verticalInputType = VerticalAxisType.Button;

        public override bool EngineStartStop()
        {
            return engineStartStopButton != null && engineStartStopButton.hasBeenClicked;
        }

        private void Update()
        {
            if (changeCameraButton != null && changeCameraButton.hasBeenClicked)
            {
                CameraChanger cameraChanger = VehicleChanger.ActiveVehicleController.gameObject
                    .GetComponentInChildren<CameraChanger>();
                if (cameraChanger != null)
                {
                    cameraChanger.NextCamera();
                }
                else
                {
                    Debug.LogError(
                        "Change camera button was pressed but the vehicle does not have CameraChanger in any of its children.");
                }
            }

            if (changeVehicleButton != null && changeVehicleButton.hasBeenClicked)
            {
                VehicleChanger.Instance.NextVehicle();
            }
        }

        public override bool ChangeCamera()
        {
            return changeCameraButton != null && changeCameraButton.hasBeenClicked;
        }

        public override bool ChangeVehicle()
        {
            return changeVehicleButton != null && changeVehicleButton.hasBeenClicked;
        }

        public override float Clutch()
        {
            // Not implemented
            return 0f;
        }

        public override bool ExtraLights()
        {
            return extraLightsButton != null && extraLightsButton.hasBeenClicked;
        }

        public override bool HighBeamLights()
        {
            return highBeamLightsButton != null && highBeamLightsButton.hasBeenClicked;
        }

        public override float Handbrake()
        {
            return handbrakeButton == null ? 0 : handbrakeButton.isPressed ? 1 : 0;
        }

        public override bool HazardLights()
        {
            return hazardLightsButton != null && hazardLightsButton.hasBeenClicked;
        }

        public override float Horizontal()
        {
            // Steering wheel input
            if (horizontalInputType == HorizontalAxisType.SteeringWheel)
            {
                if (steeringWheel != null)
                {
                    return steeringWheel.GetClampedValue();
                }

                Debug.LogWarning("HorizontalAxisType is set to SteeringWheel but no Steering Wheel has been assigned.");
            }
            // Accelerometer input
            else if (horizontalInputType == HorizontalAxisType.Accelerometer)
            {
                return UnityEngine.Input.acceleration.x * tiltSensitivity;
            }
            // Button input
            else if (horizontalInputType == HorizontalAxisType.Button)
            {
                if (steerLeftButton != null && steerRightButton != null)
                {
                    return steerLeftButton.isPressed ? -1f : steerRightButton.isPressed ? 1f : 0f;
                }

                Debug.LogWarning("HorizontalAxisType is set to button but buttons have not been assigned.");
                return 0f;
            }

            return 0;
        }

        public override bool Horn()
        {
            return hornButton != null && hornButton.hasBeenClicked;
        }

        public override bool LeftBlinker()
        {
            return leftBlinkerButton != null && leftBlinkerButton.hasBeenClicked;
        }

        public override bool LowBeamLights()
        {
            return lowBeamLightsButton != null && lowBeamLightsButton.hasBeenClicked;
        }

        public override bool RightBlinker()
        {
            return rightBlinkerButton != null && rightBlinkerButton.hasBeenClicked;
        }

        public override bool ShiftDown()
        {
            return shiftDownButton != null && shiftDownButton.hasBeenClicked;
        }

        public override int ShiftInto()
        {
            // Not implemented
            return -999;
        }

        public override bool ShiftUp()
        {
            return shiftUpButton != null && shiftUpButton.hasBeenClicked;
        }

        public override bool TrailerAttachDetach()
        {
            return trailerAttachDetachButton != null && trailerAttachDetachButton.hasBeenClicked;
        }

        public override float Vertical()
        {
            // Accelerometer input
            if (verticalInputType == VerticalAxisType.Accelerometer)
            {
                return UnityEngine.Input.acceleration.y * tiltSensitivity;
            }

            // Button input
            if (verticalInputType == VerticalAxisType.Button)
            {
                if (brakeButton != null && throttleButton != null)
                    // Brakes have priority
                {
                    return brakeButton.isPressed ? -1f : throttleButton.isPressed ? 1f : 0f;
                }

                Debug.LogWarning("VerticalAxisType is set to button but buttons have not been assigned.");
                return 0f;
            }

            return 0;
        }

        public override bool FlipOver()
        {
            return flipOverButton != null && flipOverButton.hasBeenClicked;
        }

        public override bool Boost()
        {
            return boostButton != null && boostButton.hasBeenClicked;
        }

        public override bool CruiseControl()
        {
            return cruiseControlButton != null && cruiseControlButton.hasBeenClicked;
        }
    }
}