using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.VehiclePhysics2.Input
{
    /// <summary>
    ///     Class for handling desktop user input via mouse and keyboard.
    ///     Avoid having two input managers active at the same time (mobile and destop) as the last executed script will
    ///     override the first one.
    /// </summary>
    [DisallowMultipleComponent]
    public class DesktopInputProvider : InputProvider
    {
        /// <summary>
        ///     Type of input user input.
        ///     Standard - standard keyboard, joystick or gamepad input mapped through the input manager.
        ///     Mouse - uses mouse position on screen to control throttle/braking and steering.
        ///     MouseSteer - uses LMB / RMB for throttle and braking and mouse for steering.
        /// </summary>
        public enum InputType
        {
            Standard,
            Mouse
        }


        public enum VerticalInputMapping
        {
            Standard,
            ZeroToOne,
            Composite
        }

        [Tooltip("Input type. " +
                 "Standard - uses standard input manager for all the inputs. " +
                 "Mouse - uses mouse position for steering and throttle. " +
                 "MouseSteer - uses mouse position for steering, LMB and RMB for braking / throttle.")]
        public InputType inputType = InputType.Standard;

        /// <summary>
        ///     Names of input bindings for each individual gears. If you need to add more gears modify this and the corresponding
        ///     iterator in the
        ///     ShiftInto() function.
        /// </summary>
        [NonSerialized]
        [Tooltip(
            "Names of input bindings for each individual gears. If you need to add more gears modify this and the corresponding\r\niterator in the\r\nShiftInto() function.")]
        public string[] shiftInputNames =
        {
            "ShiftIntoR1",
            "ShiftInto0",
            "ShiftInto1",
            "ShiftInto2",
            "ShiftInto3",
            "ShiftInto4",
            "ShiftInto5",
            "ShiftInto6",
            "ShiftInto7"
        };

        [FormerlySerializedAs("verticalInputType")]
        [Tooltip("Vertical input type." +
                 "Standard - uses vertical axis in range of [-1, 1] where -1 is maximum braking and 1 maximum accleration." +
                 "ZeroToOne - uses vertical axis in range of [0, 1], 0 being maximum braking and 1 maximum accelerationMag." +
                 "Composite - uses separate axes, 'Accelerator' and 'Brake' to set the vertical axis value. Still uses a single vartical axis value [-1, 1] " +
                 "throughout the system so applying full brakes and gas simultaneously is not possible.")]
        public VerticalInputMapping verticalInputMapping = VerticalInputMapping.Standard;

        private string _tmpStr;

        private int _warningCount = 0;

        public override bool EngineStartStop()
        {
            return TryGetButtonDown("EngineStartStop", KeyCode.E);
        }

        public override float Clutch()
        {
            return TryGetAxis("Clutch");
        }

        public override bool ExtraLights()
        {
            return false;
        }

        public override bool ChangeCamera()
        {
            return TryGetButtonDown("ChangeCamera", KeyCode.C);
        }

        public override bool ChangeVehicle()
        {
            return TryGetButtonDown("ChangeVehicle", KeyCode.V);
        }

        public override bool HighBeamLights()
        {
            return TryGetButtonDown("HighBeamLights", KeyCode.K);
        }


        public override float Handbrake()
        {
            float handbrake = 0;
            try
            {
                handbrake = TryGetAxis("Handbrake");
            }
            catch
            {
                handbrake = UnityEngine.Input.GetKey(KeyCode.Space) ? 1f : 0f;
            }

            return handbrake;
        }

        public override bool HazardLights()
        {
            return TryGetButtonDown("HazardLights", KeyCode.J);
        }


        public override float Horizontal()
        {
            float horizontal = 0;
            if (inputType == InputType.Standard)
            {
                horizontal = TryGetAxisRaw("Horizontal");
            }
            else
            {
                horizontal = Mathf.Clamp(InputUtility.GetMouseHorizontal(), -1f, 1f);
            }

            return horizontal;
        }

        public override bool Horn()
        {
            return TryGetButton("Horn", KeyCode.H);
        }

        public override bool LeftBlinker()
        {
            return TryGetButtonDown("LeftBlinker", KeyCode.Z);
        }

        public override bool LowBeamLights()
        {
            return TryGetButtonDown("LowBeamLights", KeyCode.L);
        }

        public override bool RightBlinker()
        {
            return TryGetButtonDown("RightBlinker", KeyCode.X);
        }

        public override bool ShiftDown()
        {
            return TryGetButtonDown("ShiftDown", KeyCode.F);
        }

        /// <summary>
        ///     Used for H-shifters and direct shifting into gear on non-sequential gearboxes.
        /// </summary>
        public override int ShiftInto()
        {
            for (int i = -1; i < 7; i++)
            {
                if (TryGetButtonDown(shiftInputNames[i + 1], KeyCode.Alpha0, false))
                {
                    return i;
                }
            }

            return -999;
        }

        public override bool ShiftUp()
        {
            return TryGetButtonDown("ShiftUp", KeyCode.R);
        }

        public override bool TrailerAttachDetach()
        {
            return TryGetButtonDown("TrailerAttachDetach", KeyCode.T);
        }


        public override float Vertical()
        {
            float vertical = 0;
            if (inputType == InputType.Standard)
            {
                if (verticalInputMapping == VerticalInputMapping.Standard)
                {
                    vertical = TryGetAxisRaw("Vertical");
                }
                else if (verticalInputMapping == VerticalInputMapping.ZeroToOne)
                {
                    vertical = (Mathf.Clamp01(TryGetAxisRaw("Vertical")) - 0.5f) * 2f;
                }
                else if (verticalInputMapping == VerticalInputMapping.Composite)
                {
                    float accelerator = Mathf.Clamp01(TryGetAxisRaw("Accelerator"));
                    float brake = Mathf.Clamp01(TryGetAxisRaw("Brake"));
                    vertical = accelerator - brake;
                }
            }
            else if (inputType == InputType.Mouse)
            {
                vertical = Mathf.Clamp(InputUtility.GetMouseVertical(), -1f, 1f);
            }
            else
            {
                if (UnityEngine.Input.GetMouseButton(0))
                {
                    vertical = 1f;
                }
                else if (UnityEngine.Input.GetMouseButton(1))
                {
                    vertical = -1f;
                }
            }

            return vertical;
        }

        public override bool FlipOver()
        {
            return TryGetButtonDown("FlipOver", KeyCode.M);
        }

        public override bool Boost()
        {
            return TryGetButton("Boost", KeyCode.LeftShift);
        }

        public override bool CruiseControl()
        {
            return TryGetButtonDown("CruiseControl", KeyCode.N);
        }

        /// <summary>
        ///     Tries to get the button value through input manager, if not falls back to hardcoded default value.
        /// </summary>
        private bool TryGetButton(string buttonName, KeyCode altKey, bool showWarning = true)
        {
            try
            {
                return UnityEngine.Input.GetButton(buttonName);
            }
            catch
            {
                // Make sure warning is not spammed as some users tend to ignore the warning and never set up the input,
                // resulting in bad performance in editor.
                if (_warningCount < 100 && showWarning)
                {
                    Debug.LogWarning(buttonName +
                                     " input binding missing, falling back to default. Check Input section in manual for more info.");
                    _warningCount++;
                }

                return UnityEngine.Input.GetKey(altKey);
            }
        }

        /// <summary>
        ///     Tries to get the button value through input manager, if not falls back to hardcoded default value.
        /// </summary>
        private bool TryGetButtonDown(string buttonName, KeyCode altKey, bool showWarning = true)
        {
            try
            {
                return UnityEngine.Input.GetButtonDown(buttonName);
            }
            catch
            {
                if (_warningCount < 100 && showWarning)
                {
                    Debug.LogWarning(buttonName +
                                     " input binding missing, falling back to default. Check Input section in manual for more info.");
                    _warningCount++;
                }

                return UnityEngine.Input.GetKeyDown(altKey);
            }
        }
        
        /// <summary>
        ///     Tries to get the axis value through input manager, if not returns 0.
        /// </summary>
        private float TryGetAxis(string axisName, bool showWarning = true)
        {
            try
            {
                return UnityEngine.Input.GetAxis(axisName);
            }
            catch
            {
                if (_warningCount < 100 && showWarning)
                {
                    Debug.LogWarning(axisName +
                                     " input binding missing. Check Input section in manual for more info.");
                    _warningCount++;
                }
            }

            return 0;
        }
        
        /// <summary>
        ///     Tries to get the axis value through input manager, if not returns 0.
        /// </summary>
        private float TryGetAxisRaw(string axisName, bool showWarning = true)
        {
            try
            {
                return UnityEngine.Input.GetAxisRaw(axisName);
            }
            catch
            {
                if (_warningCount < 100 && showWarning)
                {
                    Debug.LogWarning(axisName +
                                     " input binding missing. Check Input section in manual for more info.");
                    _warningCount++;
                }
            }

            return 0;
        }
    }
}